using Microsoft.Identity.Client;

namespace RoomManagement.DTOs;

public class LoginDTO
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}