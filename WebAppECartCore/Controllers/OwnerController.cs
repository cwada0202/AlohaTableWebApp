using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppECartCore.Models;

namespace WebAppECartCore.Controllers;

public class OwnerController : Controller
{
    private readonly AlohaTableDbContext _db;

    public OwnerController(AlohaTableDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
        {
            return RedirectToAction("Index", "Account");
        }

        ViewBag.CategorySelectList = await _db.Categories
            .OrderBy(x => x.CategoryId)
            .Select(x => new SelectListItem
            {
                Text = x.CategoryName,
                Value = x.CategoryId.ToString(),
                Selected = x.CategoryId == 1
            })
            .ToListAsync();

        var items = from item in _db.Items
                    join category in _db.Categories on item.CategoryId equals category.CategoryId
                    select new ItemListModel
                    {
                        ItemId = item.ItemId,
                        ImagePath = item.ImagePath,
                        ItemName = item.ItemName,
                        ItemCode = item.ItemCode,
                        ItemPrice = item.ItemPrice,
                        Description = item.Description,
                        ItemCategory = category.CategoryName
                    };

        var model = await items.OrderBy(x => x.ItemName).ToListAsync();
        return View(model);
    }
}
