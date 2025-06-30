using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RoomManagement.DTOs;
using RoomManagement.Models;
namespace RoomManagment.Controllers;

[ApiController]
[Route("/api/v1/[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly RoomManagementContext _context;
    private readonly IConfiguration _configuration;
    public UserController(ILogger<UserController> logger, RoomManagementContext context, IConfiguration configuration)
    {
        _logger = logger;
        _context = context;
        _configuration = configuration;
    }

    [Authorize]
    [HttpGet]
    public IActionResult GetUser(int id)
    {
        try
        {
            var user = _context.Database.SqlQuery<User>(@$"
                SELECT *
                FROM Users
                WHERE Id={id}
            ").Single();

            return Ok(user);
        }
        catch (Exception)
        {
            return NotFound($"Could not find user #{id}.");
        }
    }

    [HttpPost("Login")]
    public async Task<IActionResult> CheckLogin(LoginDTO login)
    {
        try
        {
            if (login.Password != "abc123")
            {
                return BadRequest("Invalid login.");
            }
            var user = await _context.Database.SqlQuery<User>(@$"
            SELECT *
            FROM Users
            WHERE Email= {login.Email}
            ").SingleAsync();

            return Ok(CreateToken(login, user.Id));

        }
        catch (Exception)
        {
            return BadRequest("Invalid Login.");
        }

        return Ok();

    }

    private string CreateToken(LoginDTO login, int userId) {
        var jwt = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var claims = new[] {
             new Claim(ClaimTypes.Name, login.Email),
             new Claim(ClaimTypes.Sid, userId.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(jwt["ExpireMinutes"])),
            signingCredentials: creds

        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}