using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RoomManagement.Models;

[Table("Users")]
public class User
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? Email { get; set; } = "";
    public long? Phone { get; set; }
}
