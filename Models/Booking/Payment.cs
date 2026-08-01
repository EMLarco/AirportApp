using System;

namespace AirportWebApp.Models.Booking;

public class Payment
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public string UserId { get; set; } = null!;

    public string Gateway { get; set; } = null!;

    public string? ExternalTransactionId { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime CreationDate { get; set; }

    public DateTime? ConfirmationDate { get; set; }

    public string? ResponseMessage { get; set; }

    public virtual Order Order { get; set; } = null!;
}
