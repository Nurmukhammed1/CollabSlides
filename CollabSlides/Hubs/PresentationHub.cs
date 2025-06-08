using CollabSlides.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;

namespace CollabSlides.Hubs;

public class PresentationHub : Hub
{
    private readonly ApplicationDbContext _context;

    public PresentationHub(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task JoinPresentation(Guid presentationId, Guid userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"presentation_{presentationId}");

        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            await Clients.Group($"presentation_{presentationId}")
                .SendAsync("UserJoined", new { id = user.Id, nickname = user.Nickname });
        }
    }

    public async Task LeavePresentation(Guid presentationId, Guid userId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"presentation_{presentationId}");

        await Clients.Group($"presentation_{presentationId}")
            .SendAsync("UserLeft", new { userId = userId });
    }

    public async Task TextBlockUpdated(Guid presentationId, int slideIndex, TextBlock textBlock, Guid userId)
    {
        var presentation = await _context.Presentations
            .Include(p => p.PresentationUsers)
            .Include(p => p.Slides)
            .FirstOrDefaultAsync(p => p.Id == presentationId);

        if (presentation != null)
        {
            var userRole = presentation.PresentationUsers
                .FirstOrDefault(pu => pu.UserId == userId)?.Role;

            if (userRole == "creator" || userRole == "editor")
            {
                var slide = presentation.Slides.FirstOrDefault(s => s.Index == slideIndex);
                if (slide != null)
                {
                    var existingBlock = slide.Content.FirstOrDefault(tb => tb.Id == textBlock.Id);
                    if (existingBlock != null)
                    {
                        existingBlock.Content = textBlock.Content;
                        existingBlock.X = textBlock.X;
                        existingBlock.Y = textBlock.Y;
                        existingBlock.Width = textBlock.Width;
                        existingBlock.Height = textBlock.Height;
                        existingBlock.FontSize = textBlock.FontSize;
                        existingBlock.FontWeight = textBlock.FontWeight;
                        existingBlock.FontStyle = textBlock.FontStyle;
                        existingBlock.TextAlign = textBlock.TextAlign;
                    }
                    else
                    {
                        slide.Content.Add(textBlock);
                    }

                    await _context.SaveChangesAsync();
                }

                await Clients.Group($"presentation_{presentationId}")
                    .SendAsync("TextBlockUpdated", new { textBlock = textBlock });
            }
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}
