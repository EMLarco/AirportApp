using System;

namespace AirportWebApp.Models.Booking;

public class OrderDetail
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int SeatReservationId { get; set; }

    public decimal Price { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual SeatReservation SeatReservation { get; set; } = null!;
}
