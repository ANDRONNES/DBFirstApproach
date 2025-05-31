using DBFirst.DTOs;
using DBFirst.Models;
using DBFirst.Services;
using Microsoft.AspNetCore.Mvc;

namespace DBFirst.Controllers;


[ApiController]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    private readonly ITripsService _service;
    public TripsController(ITripsService tripsService)
    {
        _service = tripsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTripsAsync([FromQuery] int? page,[FromQuery]  int? pageSize,CancellationToken ct)
    {
        var trips = await _service.GetTripsInfoAsync(page,pageSize,ct);
        return Ok(trips);
    }

    [HttpPost("{idTrip}/clients")]
    public async Task<IActionResult> AddClientToTrip([FromRoute] int idTrip, [FromBody] AddClientToTripDTO dto,CancellationToken ct)
    {
        await _service.AddClientToTripAsync(dto,ct);
        return Ok(new { message = $"Client successfully added to trip {dto.TripName}." });
    }
    
}