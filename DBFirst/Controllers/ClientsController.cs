using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DBFirst.Middlewares;
using DBFirst.Models;
using DBFirst.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using LoginRequest = DBFirst.Models.LoginRequest;
using RegisterRequest = DBFirst.Models.RegisterRequest;

namespace DBFirst.Controllers;


[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    public IClientsService _service;
    private readonly IConfiguration _configuration;

    public ClientsController(IClientsService service, IConfiguration configuration)
    {
        _configuration = configuration;
        _service = service;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterClient(RegisterRequest model)
    {
        await _service.RegisterNewUserAsync(model);
        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest model)
    {
        var rfrshToken = await _service.LogIn(model);
        Claim[] userClaim = new[]
        {
            new Claim(ClaimTypes.Name, "and"),
            new Claim(ClaimTypes.Role, "admin"),
            new Claim(ClaimTypes.Role, "user")
        };
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));
        
        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        JwtSecurityToken token = new JwtSecurityToken(
            issuer: "https://localhost:5001",
            audience: "https://localhost:5001",
            claims: userClaim,
            expires: DateTime.Now.AddMinutes(10),
            signingCredentials: credentials
        );

        return Ok(new
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(token),
            refreshToken = rfrshToken
        });
    }
    
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetClientsAsync(CancellationToken ct)
    {
        var claimsFromAccessToken = User.Claims;
        var clients = await _service.GetAllClientsAsync(ct);
        return Ok(claimsFromAccessToken.ToString());
    }
    [AllowAnonymous]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id,CancellationToken ct)
    {
        await _service.DeleteClientByIdAsync(id,ct);
        return Ok(new { message = $"Client with id = {id} successfully deleted." });
    }
    
    
    [Authorize(AuthenticationSchemes = "IgnoreTokenExpirationScheme")]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenRequest refreshToken)
    {
        var refrToken = await _service.RefreshToken(refreshToken);
        
        Claim[] userClaim = new[]
        {
            new Claim(ClaimTypes.Name, "and"),
            new Claim(ClaimTypes.Role, "admin"),
            new Claim(ClaimTypes.Role, "user")
        };

        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));

        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken jwtToken = new JwtSecurityToken(
            issuer: "https://localhost:5001",
            audience: "https://localhost:5001",
            claims: userClaim,
            expires: DateTime.UtcNow.AddMinutes(10),
            signingCredentials: credentials
        );
        
        return Ok(new
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken),
            refreshToken = refrToken
        });
    }
}