using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AirportWebApp.Data;
using AirportWebApp.Models.Booking;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AirportWebApp.Controllers;

[Authorize(Roles = "Cliente")]
public class CustomerController : Controller
{
    private readonly AirportDbContext _context;

    public CustomerController(AirportDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var reservations = await _context.SeatReservations
            .Include(sr => sr.Flight)
            .Where(sr => sr.UserId == userId)
            .OrderByDescending(sr => sr.ReservationTime)
            .ToListAsync();

        var orders = await _context.Orders
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        var payments = await _context.Payments
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreationDate)
            .ToListAsync();

        var model = new CustomerOperationsViewModel
        {
            Reservations = reservations,
            Orders = orders,
            Payments = payments
        };

        return View(model);
    }
}
