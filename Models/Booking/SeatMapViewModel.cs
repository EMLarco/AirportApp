using System;
using System.Collections.Generic;
using System.Linq;
using AirportWebApp.Models.Airport;

namespace AirportWebApp.Models.Booking;

public class SeatMapViewModel
{
    public int FlightId { get; set; }

    public flight Flight { get; set; } = null!;

    public route? Route { get; set; }

    public string? AirplaneCode { get; set; }

    public string FareClass { get; set; } = null!;

    public decimal Price { get; set; }

    public List<string> AllSeats { get; set; } = new List<string>();

    public List<string> UnavailableSeats { get; set; } = new List<string>();

    public List<string> AvailableSeats =>
        AllSeats.Except(UnavailableSeats).OrderBy(s => s, StringComparer.OrdinalIgnoreCase).ToList();
}
