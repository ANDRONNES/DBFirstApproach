using DBFirst.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DBFirst.Services;

public interface ITripsService
{
    public Task<TripPagesDTO> GetTripsInfoAsync(int? page, int? pageSize,CancellationToken ct);
    public Task AddClientToTripAsync(AddClientToTripDTO dto, CancellationToken ct);
}