using DBFirst.Exceptions;
using DBFirst.Models;
using Microsoft.EntityFrameworkCore;

namespace DBFirst.Services;

public class ClientsService : IClientsService
{
    private readonly AgencyContext _context;

    public ClientsService(AgencyContext context)
    {
        _context = context;
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
}