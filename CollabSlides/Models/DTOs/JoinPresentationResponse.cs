namespace CollabSlides.Models.DTOs;

public class JoinPresentationResponse
{
    public PresentationInfo Presentation { get; set; } = null!;
    public List<SlideInfo> Slides { get; set; } = new();
    public List<UserInfo> Users { get; set; } = new();
}

public class PresentationInfo
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class SlideInfo
{
    public Guid Id { get; set; }
    public List<TextBlock> Content { get; set; } = new();
    public int Index { get; set; }
}

public class UserInfo
{
    public Guid Id { get; set; }
    public string Nickname { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}