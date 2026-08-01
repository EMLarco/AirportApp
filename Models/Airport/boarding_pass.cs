using System;
using System.Collections.Generic;

namespace AirportWebApp.Models.Airport;

/// <summary>
/// Boarding passes
/// </summary>
public partial class boarding_pass
{
    /// <summary>
    /// Ticket number
    /// </summary>
    public string ticket_no { get; set; } = null!;

    /// <summary>
    /// Flight ID
    /// </summary>
    public int flight_id { get; set; }

    /// <summary>
    /// Seat number
    /// </summary>
    public string seat_no { get; set; } = null!;

    /// <summary>
    /// Boarding pass number
    /// </summary>
    public int? boarding_no { get; set; }

    /// <summary>
    /// Boarding time
    /// </summary>
    public DateTime? boarding_time { get; set; }

    public virtual segment segment { get; set; } = null!;
}
