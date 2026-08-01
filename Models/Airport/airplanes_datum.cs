using System;
using System.Collections.Generic;

namespace AirportWebApp.Models.Airport;

/// <summary>
/// Airplanes (internal multilingual data)
/// </summary>
public partial class airplanes_datum
{
    /// <summary>
    /// Airplane code, IATA
    /// </summary>
    public string airplane_code { get; set; } = null!;

    /// <summary>
    /// Airplane model
    /// </summary>
    public string model { get; set; } = null!;

    /// <summary>
    /// Maximum flight range, km
    /// </summary>
    public int range { get; set; }

    /// <summary>
    /// Cruise speed, km/h
    /// </summary>
    public int speed { get; set; }

    public virtual ICollection<seat> seats { get; set; } = new List<seat>();
}
