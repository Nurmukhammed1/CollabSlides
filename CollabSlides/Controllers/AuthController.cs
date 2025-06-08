using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CollabSlides.Models;
using CollabSlides.Models.DTOs;

namespace CollabSlides.Controllers;


[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AuthController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nickname))
        {
            return BadRequest("Nickname is required");
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Nickname == request.Nickname);

        if (user == null)
        {
            user = new User
            {
                Id = Guid.NewGuid(),
                Nickname = request.Nickname
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        return new LoginResponse
        {
            Id = user.Id,
            Nickname = user.Nickname
        };
    }
}