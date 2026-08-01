using System;
using System.Collections.Generic;

namespace AirportWebApp.Models.Airport;

/// <summary>
/// Seats
/// </summary>
public partial class seat
{
    /// <summary>
    /// Airplane code, IATA
    /// </summary>
    public string airplane_code { get; set; } = null!;

    /// <summary>
    /// Seat number
    /// </summary>
    public string seat_no { get; set; } = null!;

    /// <summary>
    /// Travel class
    /// </summary>
    public string fare_conditions { get; set; } = null!;

    public virtual airplanes_datum airplane_codeNavigation { get; set; } = null!;
}
