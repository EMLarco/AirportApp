using System;
using AirportWebApp.Models.Airport;

namespace AirportWebApp.Models.Booking;

public class SeatReservation
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public int FlightId { get; set; }

    public string SeatNo { get; set; } = null!;

    public string FareConditions { get; set; } = null!;

    public decimal Price { get; set; }

    public DateTime ReservationTime { get; set; }

    public bool IsPaid { get; set; }

    public bool IsCancelled { get; set; }

    public virtual flight Flight { get; set; } = null!;
}
