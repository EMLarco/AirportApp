using System;
using System.Collections.Generic;
using NpgsqlTypes;

namespace AirportWebApp.Models.Airport;

/// <summary>
/// Routes
/// </summary>
public partial class route
{
    /// <summary>
    /// Route number
    /// </summary>
    public string route_no { get; set; } = null!;

    /// <summary>
    /// Period of validity
    /// </summary>
    public NpgsqlRange<DateTime> validity { get; set; }

    /// <summary>
    /// Airport of departure
    /// </summary>
    public string departure_airport { get; set; } = null!;

    /// <summary>
    /// Airport of arrival
    /// </summary>
    public string arrival_airport { get; set; } = null!;

    /// <summary>
    /// Airplane code, IATA
    /// </summary>
    public string airplane_code { get; set; } = null!;

    /// <summary>
    /// Days of week array
    /// </summary>
    public List<int> days_of_week { get; set; } = null!;

    /// <summary>
    /// Scheduled local time of departure
    /// </summary>
    public TimeOnly scheduled_time { get; set; }

    /// <summary>
    /// Estimated duration
    /// </summary>
    public TimeSpan duration { get; set; }

    public virtual airplanes_datum airplane_codeNavigation { get; set; } = null!;

    public virtual airports_datum arrival_airportNavigation { get; set; } = null!;

    public virtual airports_datum departure_airportNavigation { get; set; } = null!;
}
