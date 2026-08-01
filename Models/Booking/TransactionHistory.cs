using System;

namespace AirportWebApp.Models.Booking;

public class TransactionHistory
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public int? OrderId { get; set; }

    public int? PaymentId { get; set; }

    public string Action { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    public string? Details { get; set; }
}
