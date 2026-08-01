using System.Collections.Generic;

namespace AirportWebApp.Models.Booking;

public class CustomerOperationsViewModel
{
    public IEnumerable<SeatReservation> Reservations { get; set; } = new List<SeatReservation>();
    public IEnumerable<Order> Orders { get; set; } = new List<Order>();
    public IEnumerable<Payment> Payments { get; set; } = new List<Payment>();
}
