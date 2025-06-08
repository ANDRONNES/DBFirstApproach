using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DBFirst.Exceptions;
using DBFirst.Middlewares;
using DBFirst.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DBFirst.Services;

public class ClientsService : IClientsService
{
    private readonly AgencyContext _context;
    private readonly IConfiguration _configuration;

    public ClientsService(AgencyContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }
    public async Task DeleteClientByIdAsync(int id,CancellationToken cancellationToken)
    {
        var client = await _context.Clients.FindAsync(id,cancellationToken);
        if (client == null)
        {
            throw new NotFoundException("Client not found");
        }

        if (await _context.ClientTrips.AnyAsync(ct => ct.Idclient == id,cancellationToken))
        {
            throw new BadRequestException("Client has at least one trip");
        }
        
        _context.Clients.Remove(client);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Client>> GetAllClientsAsync(CancellationToken ct)
    {
        var clients = await _context.Clients.ToListAsync();
        return clients;
    }

    public async Task RegisterNewUserAsync(RegisterRequest model)
    {
        var hashedPasswordAndSalt = SecurityHelpers.GetHashedPasswordAndSalt(model.Password);
        if (await _context.Users.AnyAsync(u => u.Email == model.Email || u.Username == model.Username))
        {
            throw new BadRequestException("Email or Username is already in use");
        }
        var user = new User()
        {
            Username = model.Username,
            Email = model.Email,
            PasswordHash = hashedPasswordAndSalt.Item1,
            Salt = hashedPasswordAndSalt.Item2,
            RefreshToken = SecurityHelpers.GenerateRefreshToken(),
            RefreshTokenExpires = DateTime.UtcNow.AddDays(1)
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task<string> LogIn(LoginRequest model)
    {
        var user = await _context.Users.Where(u => u.Username == model.Username).FirstOrDefaultAsync();
        if (user == null)
        {
            throw new UnauthorizedException("Invalid username");
        }
        
        string passwordHashFromDb = user.PasswordHash;
        string curHashedPassword = SecurityHelpers.GetHashedPasswordWithSalt(model.Password, user.Salt);

        if (curHashedPassword != passwordHashFromDb)
        {
            throw new UnauthorizedException("Invalid password");
        }

        
        user.RefreshToken = SecurityHelpers.GenerateRefreshToken();
        user.RefreshTokenExpires = DateTime.UtcNow.AddDays(1);
        await _context.SaveChangesAsync();

        return user.RefreshToken;
    }

    public async Task<string> RefreshToken(RefreshTokenRequest refreshToken)
    {
        User user = await _context.Users.Where(u => u.RefreshToken == refreshToken.RefreshToken).FirstOrDefaultAsync();
        if (user == null)
        {
            throw new SecurityTokenException("Invalid refresh token");
        }

        if (user.RefreshTokenExpires < DateTime.Now)
        {
            throw new SecurityTokenException("Refresh token expired");
        }
        user.RefreshToken = SecurityHelpers.GenerateRefreshToken();
        user.RefreshTokenExpires = DateTime.UtcNow.AddDays(1);
        await _context.SaveChangesAsync();
        return user.RefreshToken;
    }
}