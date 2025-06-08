namespace CollabSlides.Models.DTOs;

public class CreatePresentationRequest
{
    public string Name { get; set; } = string.Empty;
    public Guid CreatorId { get; set; }
}