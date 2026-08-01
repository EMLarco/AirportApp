using System;
using System.Collections.Generic;

namespace AirportWebApp.Models.Airport;

/// <summary>
/// Flights
/// </summary>
public partial class flight
{
    /// <summary>
    /// Flight ID
    /// </summary>
    public int flight_id { get; set; }

    /// <summary>
    /// Route number
    /// </summary>
    public string route_no { get; set; } = null!;

    /// <summary>
    /// Flight status
    /// </summary>
    public string status { get; set; } = null!;

    /// <summary>
    /// Scheduled departure time
    /// </summary>
    public DateTime scheduled_departure { get; set; }

    /// <summary>
    /// Scheduled arrival time
    /// </summary>
    public DateTime scheduled_arrival { get; set; }

    /// <summary>
    /// Actual departure time
    /// </summary>
    public DateTime? actual_departure { get; set; }

    /// <summary>
    /// Actual arrival time
    /// </summary>
    public DateTime? actual_arrival { get; set; }

    public virtual ICollection<segment> segments { get; set; } = new List<segment>();
}
