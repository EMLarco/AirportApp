using System;
using System.Collections.Generic;
using NpgsqlTypes;

namespace AirportWebApp.Models.Airport;

/// <summary>
/// Airports (internal multilingual data)
/// </summary>
public partial class airports_datum
{
    /// <summary>
    /// Airport code, IATA
    /// </summary>
    public string airport_code { get; set; } = null!;

    /// <summary>
    /// Airport name
    /// </summary>
    public string airport_name { get; set; } = null!;

    /// <summary>
    /// City
    /// </summary>
    public string city { get; set; } = null!;

    /// <summary>
    /// Country
    /// </summary>
    public string country { get; set; } = null!;

    /// <summary>
    /// Airport coordinates (longitude and latitude)
    /// </summary>
    public NpgsqlPoint coordinates { get; set; }

    /// <summary>
    /// Airport time zone
    /// </summary>
    public string timezone { get; set; } = null!;
}
