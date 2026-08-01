using System;
using System.Collections.Generic;

namespace AirportWebApp.Models.Airport;

/// <summary>
/// Flight segment (leg)
/// </summary>
public partial class segment
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
    /// Travel class
    /// </summary>
    public string fare_conditions { get; set; } = null!;

    /// <summary>
    /// Travel price
    /// </summary>
    public decimal price { get; set; }

    public virtual boarding_pass? boarding_pass { get; set; }

    public virtual flight flight { get; set; } = null!;

    public virtual ticket ticket_noNavigation { get; set; } = null!;
}
