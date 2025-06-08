using System.ComponentModel.DataAnnotations;

namespace CollabSlides.Models;

public class Presentation
{
    public Guid Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public Guid CreatorId { get; set; }
    public User Creator { get; set; } = null!;
    public List<Slide> Slides { get; set; }
    public List<PresentationUser> PresentationUsers { get; set; } = new();
}