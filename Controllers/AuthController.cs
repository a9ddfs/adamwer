using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DemoManagement.Api.Data;
using DemoManagement.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DemoManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IConfiguration configuration, AppDbContext dbContext) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto request)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(x => x.Username == request.Username && x.Password == request.Password);

        if (user is null)
        {
            return Unauthorized("Invalid username or password.");
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username)
        };

        var jwtSection = configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSection["Issuer"],
            audience: jwtSection["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials);

        return Ok(new LoginResponseDto
        {
            Username = user.Username,
            Token = new JwtSecurityTokenHandler().WriteToken(token)
        });
    }
}
