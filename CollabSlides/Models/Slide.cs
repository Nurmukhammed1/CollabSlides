namespace CollabSlides.Models;
using System.Text.Json;

public class Slide
{
    public Guid Id { get; set; }
    public Guid PresentationId { get; set; }
    public Presentation Presentation { get; set; } = null!;
        
    public int Index { get; set; }
    public List<TextBlock> Content { get; set; } = new();
}
