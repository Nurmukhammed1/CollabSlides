namespace CollabSlides.Models.DTOs;

public class LoginResponse
{
    public Guid Id { get; set; }
    public string Nickname { get; set; } = string.Empty;
}