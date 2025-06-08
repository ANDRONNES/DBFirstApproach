using DBFirst.Models;

namespace DBFirst.Services;

public interface IClientsService
{
    public Task DeleteClientByIdAsync(int id,CancellationToken ct);
    public Task<List<Client>> GetAllClientsAsync(CancellationToken ct);
    public Task RegisterNewUserAsync(RegisterRequest model);
    public Task<string> LogIn(LoginRequest model);
    public Task<string> RefreshToken(RefreshTokenRequest refreshToken);
}