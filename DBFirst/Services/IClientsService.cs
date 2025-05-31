namespace DBFirst.Services;

public interface IClientsService
{
    public Task DeleteClientByIdAsync(int id,CancellationToken ct);
}