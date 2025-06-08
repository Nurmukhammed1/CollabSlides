using System.ComponentModel.DataAnnotations;

namespace CollabSlides.Models;

public class User
{
    public Guid Id { get; set; }
    [Required]
    public string Nickname { get; set; }
    public List<Presentation> CreatedPresentations { get; set; } = new();
    public List<PresentationUser> PresentationUsers { get; set; } = new();
}