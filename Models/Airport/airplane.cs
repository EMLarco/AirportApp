using System;
using System.Collections.Generic;

namespace AirportWebApp.Models.Airport;

public partial class airplane
{
    /// <summary>
    /// Airplane code, IATA
    /// </summary>
    public string? airplane_code { get; set; }

    /// <summary>
    /// Airplane model
    /// </summary>
    public string? model { get; set; }

    /// <summary>
    /// Maximum flight range, km
    /// </summary>
    public int? range { get; set; }

    /// <summary>
    /// Cruise speed, km/h
    /// </summary>
    public int? speed { get; set; }
}
