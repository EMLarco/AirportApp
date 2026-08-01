using System;
using System.Collections.Generic;

namespace AirportWebApp.Models.Booking;

public class Order
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public DateTime OrderDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = null!;

    public string? PayPalOrderId { get; set; }

    public string? PayPalCaptureId { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
