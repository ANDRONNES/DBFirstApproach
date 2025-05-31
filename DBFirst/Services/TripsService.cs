using DBFirst.DTOs;
using DBFirst.Exceptions;
using DBFirst.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DBFirst.Services;

public class TripsService : ITripsService
{
    private readonly AgencyContext _context;

    public TripsService(AgencyContext context)
    {
        _context = context;
    }

    public async Task<TripPagesDTO> GetTripsInfoAsync(int? page, int? pageSize,CancellationToken cancellationToken)
    {
        int PageNum = page ?? 1;
        int PageSize = pageSize ?? 3;


        var TripsList =  _context.Trips.OrderByDescending(t => t.Datefrom)
            .Select(t => new TripDTO()
            {
                Name = t.Name,
                Description = t.Description,
                DateFrom = t.Datefrom,
                DateTo = t.Dateto,
                MaxPeople = t.Maxpeople,
                Countries = t.Idcountries.Select(c => new CountryDTO()
                {
                    Name = c.Name,
                }).ToList(),
                Clients = t.ClientTrips.Select(cl => new ClientDTO()
                {
                    FirstName = cl.IdclientNavigation.Firstname,
                    LastName = cl.IdclientNavigation.Lastname,
                }).ToList()
            });
        int countOfTrips = await TripsList.CountAsync(cancellationToken);

        var pagedTrips = await TripsList
            .Skip((PageNum - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync(cancellationToken);

        return new TripPagesDTO()
        {
            pageNum = PageNum,
            pageSize = PageSize,
            allPages = (int)Math.Ceiling((double)countOfTrips / PageSize),
            trips = pagedTrips
        };
    }

    public async Task AddClientToTripAsync(AddClientToTripDTO dto,CancellationToken cancellationToken)
    {
        var maxPeopleCountThisTrip = await _context.Trips
            .Where(t => t.Idtrip == dto.IdTrip)
            .Select(t => t.Maxpeople)
            .FirstOrDefaultAsync(cancellationToken);
        var currPeopleCount = await _context.ClientTrips
            .CountAsync(ct => ct.Idtrip == dto.IdTrip,cancellationToken);
        if (currPeopleCount + 1 > maxPeopleCountThisTrip)
        {
            throw new ConflictException($"Trip {dto.TripName} is full. Max people count is {maxPeopleCountThisTrip}");
        }

        if (!await _context.Clients.AnyAsync(c => c.Pesel == dto.Pesel,cancellationToken))
        {
            throw new NotFoundException("Client not found");
        }

        var client = await _context.Clients
            .SingleOrDefaultAsync(c => c.Pesel == dto.Pesel,cancellationToken);

        if (await _context.ClientTrips
                .AnyAsync(ct => ct.Idclient == client.Idclient && ct.Idtrip == dto.IdTrip,cancellationToken))
        {
            throw new ConflictException($"Client with Pesel = {dto.Pesel} is already on this trip");
        }

        if (!await _context.Trips.AnyAsync(t => t.Idtrip == dto.IdTrip,cancellationToken))
        {
            throw new NotFoundException("Trip not found");
        }

        var tripDate = await _context.Trips
            .Where(t => t.Idtrip == dto.IdTrip)
            .Select(t => t.Datefrom)
            .SingleOrDefaultAsync(cancellationToken);


        if (tripDate < DateTime.Now)
        {
            throw new ConflictException("The trip has already ended");
        }

        _context.ClientTrips.Add(new ClientTrip()
        {
            Idclient = client.Idclient,
            Idtrip = dto.IdTrip,
            Registeredat = DateTime.Now,
            Paymentdate = dto.PaymentDate,
        });
        
        await _context.SaveChangesAsync(cancellationToken);
    }
}