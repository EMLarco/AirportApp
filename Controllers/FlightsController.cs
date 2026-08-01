using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AirportWebApp.Data;
using AirportWebApp.Models.Airport;
using AirportWebApp.Models.Booking;
using AirportWebApp.Services;

namespace AirportWebApp.Controllers;

[Authorize]
public class FlightsController : Controller
{
    private readonly AirportDbContext _context;
    private readonly PayPalService _payPal;

    public FlightsController(AirportDbContext context, PayPalService payPal)
    {
        _context = context;
        _payPal = payPal;
    }

    // GET: /Flights?origin=...&destination=...&date=...&page=...
    [AllowAnonymous]
    public async Task<IActionResult> Index(
        string? origin = null,
        string? destination = null,
        DateTime? date = null,
        int page = 1,
        int pageSize = 10)
    {
        var query = _context.flights
            .AsNoTracking()
            .Where(f => f.status == "Scheduled");

        if (!string.IsNullOrEmpty(origin) || !string.IsNullOrEmpty(destination))
        {
            var routesQuery = _context.routes.AsNoTracking();
            if (!string.IsNullOrEmpty(origin))
                routesQuery = routesQuery.Where(r => r.departure_airport == origin);
            if (!string.IsNullOrEmpty(destination))
                routesQuery = routesQuery.Where(r => r.arrival_airport == destination);

            var routeNos = await routesQuery.Select(r => r.route_no).Distinct().ToListAsync();
            if (routeNos.Count == 0)
                routeNos.Add("__none__");

            query = query.Where(f => routeNos.Contains(f.route_no));
        }

        if (date.HasValue)
        {
            var start = date.Value.Date;
            var end = start.AddDays(1);
            query = query.Where(f => f.scheduled_departure >= start && f.scheduled_departure < end);
        }

        var total = await query.CountAsync();
        var flights = await query
            .OrderBy(f => f.scheduled_departure)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var pageRouteNos = flights.Select(f => f.route_no).Distinct().ToList();
        var pageRoutes = await _context.routes
            .AsNoTracking()
            .Where(r => pageRouteNos.Contains(r.route_no))
            .ToListAsync();
        var routes = pageRoutes
            .GroupBy(r => r.route_no)
            .ToDictionary(g => g.Key, g => g.First());

        var airportCodes = routes.Values
            .SelectMany(r => new[] { r.departure_airport, r.arrival_airport })
            .Distinct()
            .ToList();
        var airports = airportCodes.Count > 0
            ? await _context.airports
                .AsNoTracking()
                .Where(a => airportCodes.Contains(a.airport_code!))
                .ToDictionaryAsync(a => a.airport_code!, a => a)
            : new Dictionary<string, airport>();

        ViewBag.Airports = airports;
        ViewBag.Origin = origin;
        ViewBag.Destination = destination;
        ViewBag.Date = date?.ToString("yyyy-MM-dd");
        ViewBag.Total = total;
        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
        ViewBag.Routes = routes;

        return View(flights);
    }

    // GET: /Flights/Details/5
    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var flight = await _context.flights
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.flight_id == id);

        if (flight == null)
            return NotFound();

        var routes = await _context.routes
            .AsNoTracking()
            .Where(r => r.route_no == flight.route_no)
            .ToListAsync();
        var route = routes.FirstOrDefault();

        airplane? airplane = null;
        var airports = new Dictionary<string, airport>();
        if (route != null)
        {
            airplane = await _context.airplanes
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.airplane_code == route.airplane_code);

            var codes = new[] { route.departure_airport, route.arrival_airport };
            airports = await _context.airports
                .AsNoTracking()
                .Where(a => codes.Contains(a.airport_code))
                .ToDictionaryAsync(a => a.airport_code!, a => a);
        }

        ViewBag.Route = route;
        ViewBag.Airplane = airplane;
        ViewBag.Airports = airports;

        return View(flight);
    }

    // GET: /Flights/Seats/5?fareClass=Economy
    [AllowAnonymous]
    [Route("Flights/Seats/{flightId:int}")]
    public async Task<IActionResult> Seats(int flightId, string fareClass = "Economy")
    {
        var flight = await _context.flights
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.flight_id == flightId);

        if (flight == null)
            return NotFound();

        var routes = await _context.routes
            .AsNoTracking()
            .Where(r => r.route_no == flight.route_no)
            .ToListAsync();
        var route = routes.FirstOrDefault();
        var airplaneCode = route?.airplane_code;

        var allSeats = new List<string>();
        if (!string.IsNullOrEmpty(airplaneCode))
        {
            allSeats = await _context.seats
                .AsNoTracking()
                .Where(s => s.airplane_code == airplaneCode && s.fare_conditions == fareClass)
                .Select(s => s.seat_no)
                .ToListAsync();
        }

        var occupiedSeats = await _context.boarding_passes
            .AsNoTracking()
            .Where(bp => bp.flight_id == flightId)
            .Select(bp => bp.seat_no)
            .ToListAsync();

        var reservedSeats = await _context.SeatReservations
            .AsNoTracking()
            .Where(sr => sr.FlightId == flightId && !sr.IsCancelled && !sr.IsPaid)
            .Select(sr => sr.SeatNo)
            .ToListAsync();

        var unavailable = occupiedSeats.Union(reservedSeats).Distinct().OrderBy(s => s).ToList();

        var airports = new Dictionary<string, airport>();
        if (route != null)
        {
            var codes = new[] { route.departure_airport, route.arrival_airport };
            airports = await _context.airports
                .AsNoTracking()
                .Where(a => codes.Contains(a.airport_code!))
                .ToDictionaryAsync(a => a.airport_code!, a => a);
        }
        ViewBag.Airports = airports;

        var model = new SeatMapViewModel
        {
            FlightId = flightId,
            Flight = flight,
            Route = route,
            AirplaneCode = airplaneCode,
            FareClass = fareClass,
            Price = PriceForClass(fareClass),
            AllSeats = allSeats,
            UnavailableSeats = unavailable
        };

        return View(model);
    }

    // POST: /Flights/Reserve
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reserve(int flightId, string seatNo, string fareConditions, decimal price)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var flightExists = await _context.flights.AnyAsync(f => f.flight_id == flightId);
        if (!flightExists)
            return NotFound();

        var occupied = await _context.boarding_passes
            .AnyAsync(bp => bp.flight_id == flightId && bp.seat_no == seatNo);

        var reserved = await _context.SeatReservations
            .AnyAsync(sr => sr.FlightId == flightId && sr.SeatNo == seatNo && !sr.IsCancelled && !sr.IsPaid);

        if (occupied || reserved)
        {
            TempData["Error"] = "El asiento seleccionado ya no está disponible.";
            return RedirectToAction("Seats", new { flightId, fareClass = fareConditions });
        }

        var reservation = new SeatReservation
        {
            UserId = userId,
            FlightId = flightId,
            SeatNo = seatNo,
            FareConditions = fareConditions,
            Price = price,
            ReservationTime = DateTime.UtcNow,
            IsPaid = false,
            IsCancelled = false
        };

        _context.SeatReservations.Add(reservation);
        await _context.SaveChangesAsync();

        return RedirectToAction("Confirm", new { reservationId = reservation.Id });
    }

    // GET: /Flights/Confirm/5
    public async Task<IActionResult> Confirm(int reservationId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var reservation = await _context.SeatReservations
            .Include(sr => sr.Flight)
            .FirstOrDefaultAsync(sr => sr.Id == reservationId && sr.UserId == userId);

        if (reservation == null)
            return NotFound();

        var routes = await _context.routes
            .AsNoTracking()
            .Where(r => r.route_no == reservation.Flight.route_no)
            .ToListAsync();

        ViewBag.Reservation = reservation;
        ViewBag.Route = routes.FirstOrDefault();
        ViewBag.Total = reservation.Price;

        return View();
    }

    // POST: /Flights/Pay
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Pay(int reservationId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var reservation = await _context.SeatReservations
            .Include(sr => sr.Flight)
            .FirstOrDefaultAsync(sr => sr.Id == reservationId && sr.UserId == userId);

        if (reservation == null)
            return NotFound();

        if (reservation.IsPaid)
            return RedirectToAction("PaymentSuccess", new { reservationId });

        var total = reservation.Price;

        var order = new Order
        {
            UserId = userId,
            OrderDate = DateTime.UtcNow,
            TotalAmount = total,
            Status = "Pending"
        };
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        var detail = new OrderDetail
        {
            OrderId = order.Id,
            SeatReservationId = reservation.Id,
            Price = total
        };
        _context.OrderDetails.Add(detail);
        await _context.SaveChangesAsync();

        var returnUrl = Url.Action("PaymentSuccess", "Flights", new { reservationId }, Request.Scheme) ?? "/";
        var cancelUrl = Url.Action("PaymentCancel", "Flights", new { reservationId }, Request.Scheme) ?? "/";

        var paypalOrder = await _payPal.CreateOrder(total, returnUrl, cancelUrl);

        order.PayPalOrderId = paypalOrder.Id;
        await _context.SaveChangesAsync();

        var approvalLink = paypalOrder.Links?.FirstOrDefault(l => l.Rel == "approve")?.Href;
        if (string.IsNullOrEmpty(approvalLink))
            return StatusCode(500, "No se pudo obtener el enlace de aprobación de PayPal.");

        return Redirect(approvalLink);
    }

    // GET: /Flights/PaymentSuccess?token=...&reservationId=...
    public async Task<IActionResult> PaymentSuccess(string? token, string? PayerID, int reservationId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var reservation = await _context.SeatReservations
            .Include(sr => sr.Flight)
            .FirstOrDefaultAsync(sr => sr.Id == reservationId && sr.UserId == userId);

        if (reservation == null)
            return NotFound();

        if (reservation.IsPaid)
            return View("PaymentSuccess", reservation);

        var order = await _context.Orders
            .Include(o => o.OrderDetails)
            .FirstOrDefaultAsync(o => o.OrderDetails.Any(od => od.SeatReservationId == reservationId));

        if (order == null)
            return NotFound();

        try
        {
            var capture = await _payPal.CaptureOrder(token ?? order.PayPalOrderId ?? "");
            if (capture.Status == "COMPLETED")
            {
                order.Status = "Paid";
                order.PayPalCaptureId = capture.Id;
                reservation.IsPaid = true;
                await _context.SaveChangesAsync();

                // Prevent duplicate payment records for same external transaction
                var existingPayment = await _context.Payments.FirstOrDefaultAsync(p => p.ExternalTransactionId == capture.Id && p.Gateway == "PayPal");
                if (existingPayment == null)
                {
                    var payment = new Payment
                    {
                        OrderId = order.Id,
                        UserId = userId,
                        Gateway = "PayPal",
                        ExternalTransactionId = capture.Id,
                        Amount = order.TotalAmount,
                        Currency = "USD",
                        Status = "Approved",
                        CreationDate = DateTime.UtcNow,
                        ConfirmationDate = DateTime.UtcNow,
                        ResponseMessage = "Pago completado exitosamente"
                    };
                    _context.Payments.Add(payment);
                    await _context.SaveChangesAsync();

                    var history = new TransactionHistory
                    {
                        UserId = userId,
                        OrderId = order.Id,
                        PaymentId = payment.Id,
                        Action = "PaymentApproved",
                        Timestamp = DateTime.UtcNow,
                        Details = $"Pago aprobado para reserva {reservationId}"
                    };
                    _context.TransactionHistories.Add(history);
                    await _context.SaveChangesAsync();
                }

                return View("PaymentSuccess", reservation);
            }
            else
            {
                order.Status = "Failed";
                await _context.SaveChangesAsync();

                var externalId = token ?? order.PayPalOrderId;
                var existingPayment = await _context.Payments.FirstOrDefaultAsync(p => p.ExternalTransactionId == externalId && p.Gateway == "PayPal");
                if (existingPayment == null)
                {
                    var payment = new Payment
                    {
                        OrderId = order.Id,
                        UserId = userId,
                        Gateway = "PayPal",
                        ExternalTransactionId = externalId,
                        Amount = order.TotalAmount,
                        Currency = "USD",
                        Status = "Failed",
                        CreationDate = DateTime.UtcNow,
                        ConfirmationDate = DateTime.UtcNow,
                        ResponseMessage = "El pago no fue completado"
                    };
                    _context.Payments.Add(payment);
                    await _context.SaveChangesAsync();
                }

                return View("PaymentFailed");
            }
        }
        catch (Exception ex)
        {
            ViewBag.Error = ex.Message;
            return View("PaymentFailed");
        }
    }

    // GET: /Flights/PaymentCancel?reservationId=...
    public IActionResult PaymentCancel(int reservationId)
    {
        return View("PaymentCancel");
    }

    private static decimal PriceForClass(string fareClass)
    {
        return fareClass.ToLower() switch
        {
            "business" => 300m,
            "comfort" => 200m,
            _ => 100m
        };
    }
}
