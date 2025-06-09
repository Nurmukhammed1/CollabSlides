using CollabSlides.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;

namespace CollabSlides.Hubs;

public class TextBlockUpdateData
{
    public Guid PresentationId { get; set; }
    public int SlideIndex { get; set; }
    public TextBlock TextBlock { get; set; }
    public Guid UserId { get; set; }
}

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

    public async Task UpdateTextBlock(TextBlockUpdateData data)
    {
        var presentation = await _context.Presentations
            .Include(p => p.PresentationUsers)
            .Include(p => p.Slides)
            .FirstOrDefaultAsync(p => p.Id == data.PresentationId);

        if (presentation != null)
        {
            var userRole = presentation.PresentationUsers
                .FirstOrDefault(pu => pu.UserId == data.UserId)?.Role;

            if (userRole == "creator" || userRole == "editor")
            {
                var slide = presentation.Slides.FirstOrDefault(s => s.Index == data.SlideIndex);
                if (slide != null)
                {
                    var existingBlock = slide.Content.FirstOrDefault(tb => tb.Id == data.TextBlock.Id);
                    if (existingBlock != null)
                    {
                        existingBlock.Content = data.TextBlock.Content;
                        existingBlock.X = data.TextBlock.X;
                        existingBlock.Y = data.TextBlock.Y;
                        existingBlock.Width = data.TextBlock.Width;
                        existingBlock.Height = data.TextBlock.Height;
                        existingBlock.FontSize = data.TextBlock.FontSize;
                        existingBlock.FontWeight = data.TextBlock.FontWeight;
                        existingBlock.FontStyle = data.TextBlock.FontStyle;
                        existingBlock.TextAlign = data.TextBlock.TextAlign;
                    }
                    else
                    {
                        slide.Content.Add(data.TextBlock);
                    }

                    await _context.SaveChangesAsync();
                }
                
                await Clients.GroupExcept($"presentation_{data.PresentationId}", Context.ConnectionId)
                    .SendAsync("TextBlockUpdated", new { textBlock = data.TextBlock });
            }
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}