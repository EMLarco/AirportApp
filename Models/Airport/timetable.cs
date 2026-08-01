using System;
using System.Collections.Generic;

namespace AirportWebApp.Models.Airport;

public partial class timetable
{
    /// <summary>
    /// Flight ID
    /// </summary>
    public int? flight_id { get; set; }

    /// <summary>
    /// Route number
    /// </summary>
    public string? route_no { get; set; }

    /// <summary>
    /// Airport of departure
    /// </summary>
    public string? departure_airport { get; set; }

    /// <summary>
    /// Airport of arrival
    /// </summary>
    public string? arrival_airport { get; set; }

    /// <summary>
    /// Flight status
    /// </summary>
    public string? status { get; set; }

    /// <summary>
    /// Airplane code, IATA
    /// </summary>
    public string? airplane_code { get; set; }

    /// <summary>
    /// Scheduled departure time
    /// </summary>
    public DateTime? scheduled_departure { get; set; }

    /// <summary>
    /// Scheduled departure time in airport&apos;s timezone
    /// </summary>
    public DateTime? scheduled_departure_local { get; set; }

    /// <summary>
    /// Actual departure time
    /// </summary>
    public DateTime? actual_departure { get; set; }

    /// <summary>
    /// Actual departure time in airport&apos;s timezone
    /// </summary>
    public DateTime? actual_departure_local { get; set; }

    /// <summary>
    /// Scheduled arrival time
    /// </summary>
    public DateTime? scheduled_arrival { get; set; }

    /// <summary>
    /// Scheduled arrival time in airport&apos;s timezone
    /// </summary>
    public DateTime? scheduled_arrival_local { get; set; }

    /// <summary>
    /// Actual arrival time
    /// </summary>
    public DateTime? actual_arrival { get; set; }

    /// <summary>
    /// Actual arrival time in airport&apos;s timezone
    /// </summary>
    public DateTime? actual_arrival_local { get; set; }
}
