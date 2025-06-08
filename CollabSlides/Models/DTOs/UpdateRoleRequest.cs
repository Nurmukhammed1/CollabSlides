namespace CollabSlides.Models.DTOs;

public class UpdateRoleRequest
{
    public string Role { get; set; } = string.Empty;
    public Guid RequesterId { get; set; }
}