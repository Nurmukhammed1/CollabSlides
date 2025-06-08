namespace CollabSlides.Models.DTOs;

public class SavePresentationRequest
{
    public List<SlideInfo> Slides { get; set; } = new();
    public Guid UserId { get; set; }
}