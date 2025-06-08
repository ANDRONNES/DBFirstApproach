using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DBFirst.Models;

public partial class AgencyContext : DbContext
{
    public AgencyContext()
    {
    }

    public AgencyContext(DbContextOptions<AgencyContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<ClientTrip> ClientTrips { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Trip> Trips { get; set; }
    public virtual DbSet<User> Users { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users"); 
            entity.HasKey(e => e.IdUser);

            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.Username).HasColumnName("username");
            entity.Property(e => e.Email).HasColumnName("email");  
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.Salt).HasColumnName("salt");
            entity.Property(e => e.RefreshToken).HasColumnName("refresh_token");
            entity.Property(e => e.RefreshTokenExpires).HasColumnName("refresh_token_expires");
        });
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Idclient).HasName("client_pkey");

            entity.ToTable("client");

            entity.Property(e => e.Idclient)
                .UseIdentityAlwaysColumn()
                .HasColumnName("idclient");
            entity.Property(e => e.Email)
                .HasMaxLength(120)
                .HasColumnName("email");
            entity.Property(e => e.Firstname)
                .HasMaxLength(120)
                .HasColumnName("firstname");
            entity.Property(e => e.Lastname)
                .HasMaxLength(120)
                .HasColumnName("lastname");
            entity.Property(e => e.Pesel)
                .HasMaxLength(120)
                .HasColumnName("pesel");
            entity.Property(e => e.Telephone)
                .HasMaxLength(120)
                .HasColumnName("telephone");
        });

        modelBuilder.Entity<ClientTrip>(entity =>
        {
            entity.HasKey(e => new { e.Idclient, e.Idtrip }).HasName("client_trip_pkey");

            entity.ToTable("client_trip");

            entity.Property(e => e.Idclient).HasColumnName("idclient");
            entity.Property(e => e.Idtrip).HasColumnName("idtrip");
            entity.Property(e => e.Paymentdate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("paymentdate");
            entity.Property(e => e.Registeredat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("registeredat");

            entity.HasOne(d => d.IdclientNavigation).WithMany(p => p.ClientTrips)
                .HasForeignKey(d => d.Idclient)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("client_trip_idclient_fkey");

            entity.HasOne(d => d.IdtripNavigation).WithMany(p => p.ClientTrips)
                .HasForeignKey(d => d.Idtrip)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("client_trip_idtrip_fkey");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Idcountry).HasName("country_pkey");

            entity.ToTable("country");

            entity.Property(e => e.Idcountry)
                .UseIdentityAlwaysColumn()
                .HasColumnName("idcountry");
            entity.Property(e => e.Name)
                .HasMaxLength(120)
                .HasColumnName("name");

            entity.HasMany(d => d.Idtrips).WithMany(p => p.Idcountries)
                .UsingEntity<Dictionary<string, object>>(
                    "CountryTrip",
                    r => r.HasOne<Trip>().WithMany()
                        .HasForeignKey("Idtrip")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("country_trip_idtrip_fkey"),
                    l => l.HasOne<Country>().WithMany()
                        .HasForeignKey("Idcountry")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("country_trip_idcountry_fkey"),
                    j =>
                    {
                        j.HasKey("Idcountry", "Idtrip").HasName("country_trip_pkey");
                        j.ToTable("country_trip");
                        j.IndexerProperty<int>("Idcountry").HasColumnName("idcountry");
                        j.IndexerProperty<int>("Idtrip").HasColumnName("idtrip");
                    });
        });

        modelBuilder.Entity<Trip>(entity =>
        {
            entity.HasKey(e => e.Idtrip).HasName("trip_pkey");

            entity.ToTable("trip");

            entity.Property(e => e.Idtrip)
                .UseIdentityAlwaysColumn()
                .HasColumnName("idtrip");
            entity.Property(e => e.Datefrom)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("datefrom");
            entity.Property(e => e.Dateto)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("dateto");
            entity.Property(e => e.Description)
                .HasMaxLength(220)
                .HasColumnName("description");
            entity.Property(e => e.Maxpeople).HasColumnName("maxpeople");
            entity.Property(e => e.Name)
                .HasMaxLength(120)
                .HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
