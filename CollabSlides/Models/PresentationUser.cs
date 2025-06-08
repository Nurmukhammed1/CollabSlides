namespace CollabSlides.Models;

public class PresentationUser
{
    public Guid PresentationId { get; set; }
    public Presentation Presentation { get; set; } = null!;
        
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
        
    public string Role { get; set; } = "viewer"; // creator, editor, viewer
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}