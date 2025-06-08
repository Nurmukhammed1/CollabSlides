namespace CollabSlides.Models.DTOs;

public class CreateSlideRequest
{
    public Guid UserId { get; set; }
    public int Index { get; set; }
}