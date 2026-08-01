using System;
using System.Collections.Generic;
using AirportWebApp.Models;
using AirportWebApp.Models.Airport;
using AirportWebApp.Models.Booking;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace AirportWebApp.Data;

public partial class AirportDbContext : IdentityDbContext<ApplicationUser>
{
    public AirportDbContext()
    {
    }

    public AirportDbContext(DbContextOptions<AirportDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<airplane> airplanes { get; set; }

    public virtual DbSet<airplanes_datum> airplanes_data { get; set; }

    public virtual DbSet<airport> airports { get; set; }

    public virtual DbSet<airports_datum> airports_data { get; set; }

    public virtual DbSet<boarding_pass> boarding_passes { get; set; }

    public virtual DbSet<booking> bookings { get; set; }

    public virtual DbSet<flight> flights { get; set; }

    public virtual DbSet<route> routes { get; set; }

    public virtual DbSet<seat> seats { get; set; }

    public virtual DbSet<segment> segments { get; set; }

    public virtual DbSet<ticket> tickets { get; set; }

    public virtual DbSet<timetable> timetables { get; set; }

    public virtual DbSet<SeatReservation> SeatReservations { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<TransactionHistory> TransactionHistories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Leave configuration to the application's DI (AddDbContext in Program.cs).
            return;
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .UseCollation("en_US.UTF-8")
            .HasPostgresExtension("btree_gist")
            .HasPostgresExtension("cube")
            .HasPostgresExtension("earthdistance");

        modelBuilder.Entity<airplane>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("airplanes", "bookings");

            entity.Property(e => e.airplane_code)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasComment("Airplane code, IATA");
            entity.Property(e => e.model).HasComment("Airplane model");
            entity.Property(e => e.range).HasComment("Maximum flight range, km");
            entity.Property(e => e.speed).HasComment("Cruise speed, km/h");
        });

        modelBuilder.Entity<airplanes_datum>(entity =>
        {
            entity.HasKey(e => e.airplane_code).HasName("airplanes_data_pkey");

            entity.ToTable("airplanes_data", "bookings", tb => tb.HasComment("Airplanes (internal multilingual data)"));

            entity.Property(e => e.airplane_code)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasComment("Airplane code, IATA");
            entity.Property(e => e.model)
                .HasComment("Airplane model")
                .HasColumnType("jsonb");
            entity.Property(e => e.range).HasComment("Maximum flight range, km");
            entity.Property(e => e.speed).HasComment("Cruise speed, km/h");
        });

        modelBuilder.Entity<airport>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("airports", "bookings");

            entity.Property(e => e.airport_code)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasComment("Airport code, IATA");
            entity.Property(e => e.airport_name).HasComment("Airport name");
            entity.Property(e => e.city).HasComment("City");
            entity.Property(e => e.coordinates).HasComment("Airport coordinates (longitude and latitude)");
            entity.Property(e => e.country).HasComment("Country");
            entity.Property(e => e.timezone).HasComment("Airport time zone");
        });

        modelBuilder.Entity<airports_datum>(entity =>
        {
            entity.HasKey(e => e.airport_code).HasName("airports_data_pkey");

            entity.ToTable("airports_data", "bookings", tb => tb.HasComment("Airports (internal multilingual data)"));

            entity.Property(e => e.airport_code)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasComment("Airport code, IATA");
            entity.Property(e => e.airport_name)
                .HasComment("Airport name")
                .HasColumnType("jsonb");
            entity.Property(e => e.city)
                .HasComment("City")
                .HasColumnType("jsonb");
            entity.Property(e => e.coordinates).HasComment("Airport coordinates (longitude and latitude)");
            entity.Property(e => e.country)
                .HasComment("Country")
                .HasColumnType("jsonb");
            entity.Property(e => e.timezone).HasComment("Airport time zone");
        });

        modelBuilder.Entity<boarding_pass>(entity =>
        {
            entity.HasKey(e => new { e.ticket_no, e.flight_id }).HasName("boarding_passes_pkey");

            entity.ToTable("boarding_passes", "bookings", tb => tb.HasComment("Boarding passes"));

            entity.HasIndex(e => new { e.flight_id, e.boarding_no }, "boarding_passes_flight_id_boarding_no_key").IsUnique();

            entity.HasIndex(e => new { e.flight_id, e.seat_no }, "boarding_passes_flight_id_seat_no_key").IsUnique();

            entity.Property(e => e.ticket_no).HasComment("Ticket number");
            entity.Property(e => e.flight_id).HasComment("Flight ID");
            entity.Property(e => e.boarding_no).HasComment("Boarding pass number");
            entity.Property(e => e.boarding_time).HasComment("Boarding time");
            entity.Property(e => e.seat_no).HasComment("Seat number");

            entity.HasOne(d => d.segment).WithOne(p => p.boarding_pass)
                .HasForeignKey<boarding_pass>(d => new { d.ticket_no, d.flight_id })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("boarding_passes_ticket_no_flight_id_fkey");
        });

        modelBuilder.Entity<booking>(entity =>
        {
            entity.HasKey(e => e.book_ref).HasName("bookings_pkey");

            entity.ToTable("bookings", "bookings", tb => tb.HasComment("Bookings"));

            entity.Property(e => e.book_ref)
                .HasMaxLength(6)
                .IsFixedLength()
                .HasComment("Booking number");
            entity.Property(e => e.book_date).HasComment("Booking date");
            entity.Property(e => e.total_amount)
                .HasPrecision(10, 2)
                .HasComment("Total booking amount");
        });

        modelBuilder.Entity<flight>(entity =>
        {
            entity.HasKey(e => e.flight_id).HasName("flights_pkey");

            entity.ToTable("flights", "bookings", tb => tb.HasComment("Flights"));

            entity.HasIndex(e => new { e.route_no, e.scheduled_departure }, "flights_route_no_scheduled_departure_key").IsUnique();

            entity.Property(e => e.flight_id)
                .HasComment("Flight ID")
                .UseIdentityAlwaysColumn();
            entity.Property(e => e.actual_arrival).HasComment("Actual arrival time");
            entity.Property(e => e.actual_departure).HasComment("Actual departure time");
            entity.Property(e => e.route_no).HasComment("Route number");
            entity.Property(e => e.scheduled_arrival).HasComment("Scheduled arrival time");
            entity.Property(e => e.scheduled_departure).HasComment("Scheduled departure time");
            entity.Property(e => e.status).HasComment("Flight status");
        });

        modelBuilder.Entity<route>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("routes", "bookings", tb => tb.HasComment("Routes"));

            entity.HasIndex(e => new { e.route_no, e.validity }, "routes_route_no_validity_excl").HasMethod("gist");

            entity.Property(e => e.airplane_code)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasComment("Airplane code, IATA");
            entity.Property(e => e.arrival_airport)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasComment("Airport of arrival");
            entity.Property(e => e.days_of_week).HasComment("Days of week array");
            entity.Property(e => e.departure_airport)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasComment("Airport of departure");
            entity.Property(e => e.duration).HasComment("Estimated duration");
            entity.Property(e => e.route_no).HasComment("Route number");
            entity.Property(e => e.scheduled_time).HasComment("Scheduled local time of departure");
            entity.Property(e => e.validity).HasComment("Period of validity");

            entity.HasOne(d => d.airplane_codeNavigation).WithMany()
                .HasForeignKey(d => d.airplane_code)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("routes_airplane_code_fkey");

            entity.HasOne(d => d.arrival_airportNavigation).WithMany()
                .HasForeignKey(d => d.arrival_airport)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("routes_arrival_airport_fkey");

            entity.HasOne(d => d.departure_airportNavigation).WithMany()
                .HasForeignKey(d => d.departure_airport)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("routes_departure_airport_fkey");
        });

        modelBuilder.Entity<seat>(entity =>
        {
            entity.HasKey(e => new { e.airplane_code, e.seat_no }).HasName("seats_pkey");

            entity.ToTable("seats", "bookings", tb => tb.HasComment("Seats"));

            entity.Property(e => e.airplane_code)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasComment("Airplane code, IATA");
            entity.Property(e => e.seat_no).HasComment("Seat number");
            entity.Property(e => e.fare_conditions).HasComment("Travel class");

            entity.HasOne(d => d.airplane_codeNavigation).WithMany(p => p.seats)
                .HasForeignKey(d => d.airplane_code)
                .HasConstraintName("seats_airplane_code_fkey");
        });

        modelBuilder.Entity<segment>(entity =>
        {
            entity.HasKey(e => new { e.ticket_no, e.flight_id }).HasName("segments_pkey");

            entity.ToTable("segments", "bookings", tb => tb.HasComment("Flight segment (leg)"));

            entity.HasIndex(e => e.flight_id, "segments_flight_id_idx");

            entity.Property(e => e.ticket_no).HasComment("Ticket number");
            entity.Property(e => e.flight_id).HasComment("Flight ID");
            entity.Property(e => e.fare_conditions).HasComment("Travel class");
            entity.Property(e => e.price)
                .HasPrecision(10, 2)
                .HasComment("Travel price");

            entity.HasOne(d => d.flight).WithMany(p => p.segments)
                .HasForeignKey(d => d.flight_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("segments_flight_id_fkey");

            entity.HasOne(d => d.ticket_noNavigation).WithMany(p => p.segments)
                .HasForeignKey(d => d.ticket_no)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("segments_ticket_no_fkey");
        });

        modelBuilder.Entity<ticket>(entity =>
        {
            entity.HasKey(e => e.ticket_no).HasName("tickets_pkey");

            entity.ToTable("tickets", "bookings", tb => tb.HasComment("Tickets"));

            entity.HasIndex(e => new { e.book_ref, e.passenger_id, e.outbound }, "tickets_book_ref_passenger_id_outbound_key").IsUnique();

            entity.Property(e => e.ticket_no).HasComment("Ticket number");
            entity.Property(e => e.book_ref)
                .HasMaxLength(6)
                .IsFixedLength()
                .HasComment("Booking number");
            entity.Property(e => e.outbound).HasComment("Outbound flight?");
            entity.Property(e => e.passenger_id).HasComment("Passenger ID");
            entity.Property(e => e.passenger_name).HasComment("Passenger name");

            entity.HasOne(d => d.book_refNavigation).WithMany(p => p.tickets)
                .HasForeignKey(d => d.book_ref)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tickets_book_ref_fkey");
        });

        modelBuilder.Entity<timetable>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("timetable", "bookings");

            entity.Property(e => e.actual_arrival).HasComment("Actual arrival time");
            entity.Property(e => e.actual_arrival_local)
                .HasComment("Actual arrival time in airport's timezone")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.actual_departure).HasComment("Actual departure time");
            entity.Property(e => e.actual_departure_local)
                .HasComment("Actual departure time in airport's timezone")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.airplane_code)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasComment("Airplane code, IATA");
            entity.Property(e => e.arrival_airport)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasComment("Airport of arrival");
            entity.Property(e => e.departure_airport)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasComment("Airport of departure");
            entity.Property(e => e.flight_id).HasComment("Flight ID");
            entity.Property(e => e.route_no).HasComment("Route number");
            entity.Property(e => e.scheduled_arrival).HasComment("Scheduled arrival time");
            entity.Property(e => e.scheduled_arrival_local)
                .HasComment("Scheduled arrival time in airport's timezone")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.scheduled_departure).HasComment("Scheduled departure time");
            entity.Property(e => e.scheduled_departure_local)
                .HasComment("Scheduled departure time in airport's timezone")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.status).HasComment("Flight status");
        });

        modelBuilder.Entity<SeatReservation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("seat_reservations_pkey");

            entity.ToTable("seat_reservations", "bookings");

            entity.Property(e => e.UserId).HasMaxLength(450);
            entity.Property(e => e.SeatNo).HasMaxLength(64);
            entity.Property(e => e.FareConditions).HasMaxLength(64);
            entity.Property(e => e.Price).HasPrecision(10, 2);

            entity.HasOne(d => d.Flight).WithMany()
                .HasForeignKey(d => d.FlightId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("seat_reservations_flight_id_fkey");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("orders_pkey");

            entity.ToTable("orders", "bookings");

            entity.Property(e => e.UserId).HasMaxLength(450);
            entity.Property(e => e.TotalAmount).HasPrecision(10, 2);
            entity.Property(e => e.Status).HasMaxLength(32);
            entity.Property(e => e.PayPalOrderId).HasMaxLength(64);
            entity.Property(e => e.PayPalCaptureId).HasMaxLength(64);
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("order_details_pkey");

            entity.ToTable("order_details", "bookings");

            entity.HasIndex(e => e.SeatReservationId, "order_details_seat_reservation_id_key").IsUnique();

            entity.Property(e => e.Price).HasPrecision(10, 2);

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("order_details_order_id_fkey");

            entity.HasOne(d => d.SeatReservation).WithMany()
                .HasForeignKey(d => d.SeatReservationId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("order_details_seat_reservation_id_fkey");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("payments_pkey");

            entity.ToTable("payments", "bookings");

            entity.Property(e => e.UserId).HasMaxLength(450);
            entity.Property(e => e.Gateway).HasMaxLength(32);
            entity.Property(e => e.ExternalTransactionId).HasMaxLength(64);
            entity.Property(e => e.Amount).HasPrecision(10, 2);
            entity.Property(e => e.Currency).HasMaxLength(8);
            entity.Property(e => e.Status).HasMaxLength(32);

            entity.HasOne(d => d.Order).WithMany()
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("payments_order_id_fkey");
        });

        modelBuilder.Entity<TransactionHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("transaction_history_pkey");

            entity.ToTable("transaction_history", "bookings");

            entity.Property(e => e.UserId).HasMaxLength(450);
            entity.Property(e => e.Action).HasMaxLength(64);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
