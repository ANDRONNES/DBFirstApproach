using DBFirst.Middlewares;
using DBFirst.Models;
using DBFirst.Services;
using Microsoft.EntityFrameworkCore;

namespace DBFirst;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        //Dependencies injection
        
        // Add services to the container.
        builder.Services.AddScoped<ITripsService, TripsService>();
        builder.Services.AddScoped<IClientsService, ClientsService>();
        builder.Services.AddDbContext<AgencyContext>(opt =>
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            opt.UseNpgsql(connectionString);
        });
        
        
        builder.Services.AddOpenApi();
        

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }
        app.UseGlobalExceptionHandling();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}