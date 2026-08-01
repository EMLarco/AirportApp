using System;
using System.Collections.Generic;

namespace AirportWebApp.Models.Airport;

/// <summary>
/// Tickets
/// </summary>
public partial class ticket
{
    /// <summary>
    /// Ticket number
    /// </summary>
    public string ticket_no { get; set; } = null!;

    /// <summary>
    /// Booking number
    /// </summary>
    public string book_ref { get; set; } = null!;

    /// <summary>
    /// Passenger ID
    /// </summary>
    public string passenger_id { get; set; } = null!;

    /// <summary>
    /// Passenger name
    /// </summary>
    public string passenger_name { get; set; } = null!;

    /// <summary>
    /// Outbound flight?
    /// </summary>
    public bool outbound { get; set; }

    public virtual booking book_refNavigation { get; set; } = null!;

    public virtual ICollection<segment> segments { get; set; } = new List<segment>();
}
