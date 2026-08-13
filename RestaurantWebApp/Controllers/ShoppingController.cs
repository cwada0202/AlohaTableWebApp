using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantWebApp.Models;

namespace RestaurantWebApp.Controllers;

public class ShoppingController : Controller
{
    private readonly AlohaTableDbContext _db;

    public ShoppingController(AlohaTableDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var items = await (from objItem in _db.Items
                           join objCategory in _db.Categories
                               on objItem.CategoryId equals objCategory.CategoryId
                           select new ShoppingViewModel
                           {
                               ItemId = objItem.ItemId,
                               ImagePath = objItem.ImagePath,
                               ItemName = objItem.ItemName,
                               Description = objItem.Description,
                               ItemPrice = objItem.ItemPrice,
                               Category = objCategory.CategoryName,
                               ItemCode = objItem.ItemCode
                           }).ToListAsync();

        return View(items);
    }

    [HttpPost]
    public IActionResult Index(string ItemId, int Qty)
    {
        var cartJson = HttpContext.Session.GetString("CartItem");
        var cart = string.IsNullOrEmpty(cartJson)
            ? new List<ShoppingCartModel>()
            : System.Text.Json.JsonSerializer.Deserialize<List<ShoppingCartModel>>(cartJson) ?? new List<ShoppingCartModel>();

        if (!Guid.TryParse(ItemId, out var itemGuid))
        {
            return Json(new { Success = false, Message = "Invalid item." });
        }

        var item = _db.Items.SingleOrDefault(model => model.ItemId == itemGuid);
        if (item == null)
        {
            return Json(new { Success = false, Message = "Item not found." });
        }

        var cartItem = cart.FirstOrDefault(model => model.ItemId.Equals(ItemId, StringComparison.OrdinalIgnoreCase));
        if (cartItem != null)
        {
            cartItem.Quantity = Qty;
            cartItem.Total = cartItem.Quantity * cartItem.UnitPrice;
        }
        else
        {
            cartItem = new ShoppingCartModel
            {
                ItemId = ItemId,
                ImagePath = item.ImagePath,
                ItemName = item.ItemName,
                UnitPrice = item.ItemPrice,
                Quantity = Qty,
                Total = Qty * item.ItemPrice
            };
            cart.Add(cartItem);
        }

        var totalQty = cart.Sum(x => (int)x.Quantity);
        HttpContext.Session.SetInt32("CartCounter", totalQty);
        HttpContext.Session.SetString("CartItem", System.Text.Json.JsonSerializer.Serialize(cart));

        return Json(new { Success = true, Counter = cart.Count });
    }

    public IActionResult ShoppingCart()
    {
        var cartJson = HttpContext.Session.GetString("CartItem");
        if (string.IsNullOrEmpty(cartJson))
        {
            return View(new List<ShoppingCartModel>());
        }

        var cart = System.Text.Json.JsonSerializer.Deserialize<List<ShoppingCartModel>>(cartJson);
        return View(cart ?? new List<ShoppingCartModel>());
    }

    [HttpPost]
    public IActionResult AddOrder()
    {
        var cartJson = HttpContext.Session.GetString("CartItem");
        if (string.IsNullOrEmpty(cartJson))
        {
            return RedirectToAction(nameof(ShoppingCart));
        }

        var cart = System.Text.Json.JsonSerializer.Deserialize<List<ShoppingCartModel>>(cartJson) ?? new List<ShoppingCartModel>();
        var order = new Order
        {
            OrderDate = DateTime.Now,
            OrderNumber = DateTime.Now.ToString("ddMMyyyyHHmmss")
        };

        _db.Orders.Add(order);
        _db.SaveChanges();

        foreach (var item in cart)
        {
            var orderDetail = new OrderDetail
            {
                OrderId = order.OrderId,
                ItemId = Guid.Parse(item.ItemId),
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                Total = item.Total
            };
            _db.OrderDetails.Add(orderDetail);
        }

        _db.SaveChanges();

        HttpContext.Session.Remove("CartItem");
        HttpContext.Session.Remove("CartCounter");
        HttpContext.Session.SetString("OrderComplete", "true");

        return RedirectToAction(nameof(ShoppingCart));
    }

    [HttpPost]
    public IActionResult RemoveFromCart(string ItemId)
    {
        var cartJson = HttpContext.Session.GetString("CartItem");
        if (string.IsNullOrEmpty(cartJson))
        {
            return Json(new { Success = true, Counter = 0 });
        }

        var cart = System.Text.Json.JsonSerializer.Deserialize<List<ShoppingCartModel>>(cartJson) ?? new List<ShoppingCartModel>();
        var updated = cart.Where(x => x.ItemId != ItemId).ToList();

        if (updated.Count == 0)
        {
            HttpContext.Session.Remove("CartItem");
            HttpContext.Session.Remove("CartCounter");
        }
        else
        {
            HttpContext.Session.SetString("CartItem", System.Text.Json.JsonSerializer.Serialize(updated));
            HttpContext.Session.SetInt32("CartCounter", updated.Sum(x => (int)x.Quantity));
        }

        return Json(new { Success = true, Counter = updated.Count });
    }
}

