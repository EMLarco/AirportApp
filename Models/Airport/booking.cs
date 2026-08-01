using System;
using System.Collections.Generic;

namespace AirportWebApp.Models.Airport;

/// <summary>
/// Bookings
/// </summary>
public partial class booking
{
    /// <summary>
    /// Booking number
    /// </summary>
    public string book_ref { get; set; } = null!;

    /// <summary>
    /// Booking date
    /// </summary>
    public DateTime book_date { get; set; }

    /// <summary>
    /// Total booking amount
    /// </summary>
    public decimal total_amount { get; set; }

    public virtual ICollection<ticket> tickets { get; set; } = new List<ticket>();
}
