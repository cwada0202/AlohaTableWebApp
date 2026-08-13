using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantWebApp.Models;

namespace RestaurantWebApp.Controllers;

public class OrderController : Controller
{
    private readonly AlohaTableDbContext _db;

    public OrderController(AlohaTableDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
        {
            return RedirectToAction("Index", "Account");
        }

        var records = await _db.Orders
            .Select(o => new OrderRecordModel
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                OrderNumber = o.OrderNumber,
                Total = o.OrderDetails.Sum(d => d.Total),
                ItemCount = o.OrderDetails.Count()
            })
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return View(records);
    }
}
