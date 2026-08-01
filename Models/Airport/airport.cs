using System;
using System.Collections.Generic;
using NpgsqlTypes;

namespace AirportWebApp.Models.Airport;

public partial class airport
{
    /// <summary>
    /// Airport code, IATA
    /// </summary>
    public string? airport_code { get; set; }

    /// <summary>
    /// Airport name
    /// </summary>
    public string? airport_name { get; set; }

    /// <summary>
    /// City
    /// </summary>
    public string? city { get; set; }

    /// <summary>
    /// Country
    /// </summary>
    public string? country { get; set; }

    /// <summary>
    /// Airport coordinates (longitude and latitude)
    /// </summary>
    public NpgsqlPoint? coordinates { get; set; }

    /// <summary>
    /// Airport time zone
    /// </summary>
    public string? timezone { get; set; }
}
