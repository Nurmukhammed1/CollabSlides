namespace CollabSlides.Models.DTOs;

public class PresentationListResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CreatorName { get; set; } = string.Empty;
    public int ActiveUsers { get; set; }
}