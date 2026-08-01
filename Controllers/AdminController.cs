using System.Linq;
using System.Threading.Tasks;
using AirportWebApp.Data;
using AirportWebApp.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AirportWebApp.Controllers;

[Authorize(Roles = "Administrador")]
public class AdminController : Controller
{
    private readonly AirportDbContext _context;

    public AdminController(AirportDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var model = new AdminDashboardViewModel
        {
            TotalOrders = await _context.Orders.CountAsync(),
            TotalPayments = await _context.Payments.CountAsync(),
            TotalTransactions = await _context.TransactionHistories.CountAsync(),
            TotalRevenue = await _context.Payments
                .Where(p => p.Status == "Approved")
                .SumAsync(p => (decimal?)p.Amount) ?? 0m
        };

        return View(model);
    }

    public async Task<IActionResult> Orders()
    {
        var orders = await _context.Orders
            .Include(o => o.OrderDetails)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return View(orders);
    }

    public async Task<IActionResult> Payments()
    {
        var payments = await _context.Payments
            .OrderByDescending(p => p.CreationDate)
            .ToListAsync();

        return View(payments);
    }

    public async Task<IActionResult> Transactions()
    {
        var transactions = await _context.TransactionHistories
            .OrderByDescending(t => t.Timestamp)
            .ToListAsync();

        return View(transactions);
    }
}
