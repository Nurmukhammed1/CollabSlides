namespace CollabSlides.Models;

public class TextBlock
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = "textBlock";
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string Content { get; set; } = string.Empty;
    public int FontSize { get; set; } = 16;
    public string FontWeight { get; set; } = "normal";
    public string FontStyle { get; set; } = "normal";
    public string TextAlign { get; set; } = "left";
}