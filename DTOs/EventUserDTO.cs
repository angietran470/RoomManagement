using RoomManagement.Models;

namespace RoomManagement.DTOs;

public class EventUserDTO : Event
{
    public string? UserName { get; set; }
    public long Phone { get; set; }
    public string? Email { get; set; }
}