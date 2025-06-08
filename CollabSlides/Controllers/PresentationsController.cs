using CollabSlides.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CollabSlides.Models.DTOs;
using CollabSlides.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CollabSlides.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PresentationsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<PresentationHub> _hubContext;

    public PresentationsController(ApplicationDbContext context, IHubContext<PresentationHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    [HttpGet]
    public async Task<ActionResult<List<PresentationListResponse>>> GetPresentations()
    {
        var presentations = await _context.Presentations
            .Include(p => p.Creator)
            .Include(p => p.PresentationUsers)
            .Select(p => new PresentationListResponse
            {
                Id = p.Id,
                Name = p.Name,
                CreatorName = p.Creator.Nickname,
                ActiveUsers = p.PresentationUsers.Count
            })
            .ToListAsync();

        return presentations;
    }

    [HttpPost]
    public async Task<ActionResult<Presentation>> CreatePresentation(CreatePresentationRequest request)
    {
        var user = await _context.Users.FindAsync(request.CreatorId);
        if (user == null)
        {
            return BadRequest("Creator not found");
        }

        var presentation = new Presentation
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            CreatorId = request.CreatorId
        };

        _context.Presentations.Add(presentation);

        var creatorUser = new PresentationUser
        {
            PresentationId = presentation.Id,
            UserId = request.CreatorId,
            Role = "creator"
        };

        _context.PresentationUsers.Add(creatorUser);
        await _context.SaveChangesAsync();

        var response = new PresentationListResponse
        {
            Id = presentation.Id,
            Name = presentation.Name,
            CreatorName = user.Nickname,
            ActiveUsers = 1 // The creator is the first active user
        };

        return CreatedAtAction(nameof(GetPresentations), new { id = presentation.Id }, response);
    }

    [HttpPost("{id}/join")]
    public async Task<ActionResult<JoinPresentationResponse>> JoinPresentation(Guid id, JoinPresentationRequest request)
    {
        var presentation = await _context.Presentations
            .Include(p => p.Slides)
            .Include(p => p.PresentationUsers)
                .ThenInclude(pu => pu.User)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (presentation == null)
        {
            return NotFound("Presentation not found");
        }

        var user = await _context.Users.FindAsync(request.UserId);
        if (user == null)
        {
            return BadRequest("User not found");
        }

        var existingUser = presentation.PresentationUsers
            .FirstOrDefault(pu => pu.UserId == request.UserId);

        if (existingUser == null)
        {
            var newUser = new PresentationUser
            {
                PresentationId = id,
                UserId = request.UserId,
                Role = "viewer"
            };

            _context.PresentationUsers.Add(newUser);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.Group($"presentation_{id}")
                .SendAsync("UserJoined", new
                {
                    id = user.Id,
                    nickname = user.Nickname,
                    role = "viewer"
                });
        }

        var response = new JoinPresentationResponse
        {
            Presentation = new PresentationInfo
            {
                Id = presentation.Id,
                Name = presentation.Name
            },
            Slides = presentation.Slides
                .OrderBy(s => s.Index)
                .Select(s => new SlideInfo
                {
                    Id = s.Id,
                    Content = s.Content,
                    Index = s.Index
                })
                .ToList(),
            Users = presentation.PresentationUsers
                .Select(pu => new UserInfo
                {
                    Id = pu.User.Id,
                    Nickname = pu.User.Nickname,
                    Role = pu.Role
                })
                .ToList()
        };

        return response;
    }

    [HttpPost("{id}/slides")]
    public async Task<ActionResult<SlideInfo>> CreateSlide(Guid id, CreateSlideRequest request)
    {
        var presentation = await _context.Presentations
            .Include(p => p.PresentationUsers)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (presentation == null)
        {
            return NotFound("Presentation not found");
        }

        var userRole = presentation.PresentationUsers
            .FirstOrDefault(pu => pu.UserId == request.UserId)?.Role;

        if (userRole != "creator" && userRole != "editor")
        {
            return Forbid("Insufficient permissions");
        }

        var slide = new Slide
        {
            Id = Guid.NewGuid(),
            PresentationId = id,
            Index = request.Index,
            Content = new List<TextBlock>()
        };

        _context.Slides.Add(slide);
        await _context.SaveChangesAsync();

        var slideInfo = new SlideInfo
        {
            Id = slide.Id,
            Content = new List<TextBlock>(),
            Index = slide.Index
        };

        await _hubContext.Clients.Group($"presentation_{id}")
            .SendAsync("SlideAdded", slideInfo);

        return slideInfo;
    }

    [HttpPost("{id}/save")]
    public async Task<IActionResult> SavePresentation(Guid id, SavePresentationRequest request)
    {
        var presentation = await _context.Presentations
            .Include(p => p.PresentationUsers)
            .Include(p => p.Slides)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (presentation == null)
        {
            return NotFound("Presentation not found");
        }

        var userRole = presentation.PresentationUsers
            .FirstOrDefault(pu => pu.UserId == request.UserId)?.Role;

        if (userRole != "creator" && userRole != "editor")
        {
            return Forbid("Insufficient permissions");
        }

        foreach (var slideInfo in request.Slides)
        {
            var existingSlide = presentation.Slides.FirstOrDefault(s => s.Id == slideInfo.Id);
            if (existingSlide != null)
            {
                existingSlide.Content = slideInfo.Content;
                existingSlide.Index = slideInfo.Index;
            }
        }

        await _context.SaveChangesAsync();

        foreach (var slideInfo in request.Slides)
        {
            await _hubContext.Clients.Group($"presentation_{id}")
                .SendAsync("SlideUpdated", new
                {
                    slideIndex = slideInfo.Index,
                    content = slideInfo.Content
                });
        }

        return Ok();
    }

    [HttpPut("{id}/users/{userId}/role")]
    public async Task<IActionResult> UpdateUserRole(Guid id, Guid userId, UpdateRoleRequest request)
    {
        var presentation = await _context.Presentations
            .Include(p => p.PresentationUsers)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (presentation == null)
        {
            return NotFound("Presentation not found");
        }

        var requesterRole = presentation.PresentationUsers
            .FirstOrDefault(pu => pu.UserId == request.RequesterId)?.Role;

        if (requesterRole != "creator")
        {
            return Forbid("Only creators can change user roles");
        }

        var targetUser = presentation.PresentationUsers
            .FirstOrDefault(pu => pu.UserId == userId);

        if (targetUser == null)
        {
            return NotFound("User not in presentation");
        }

        if (targetUser.Role == "creator")
        {
            return BadRequest("Cannot change creator role");
        }

        targetUser.Role = request.Role;
        await _context.SaveChangesAsync();

        await _hubContext.Clients.Group($"presentation_{id}")
            .SendAsync("UserRoleChanged", new
            {
                userId = userId,
                role = request.Role
            });

        return Ok();
    }
}
