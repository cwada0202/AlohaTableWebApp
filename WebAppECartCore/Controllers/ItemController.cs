using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppECartCore.Models;

namespace WebAppECartCore.Controllers;

public class ItemController : Controller
{
    private readonly AlohaTableDbContext _db;
    private readonly IWebHostEnvironment _environment;

    public ItemController(AlohaTableDbContext db, IWebHostEnvironment environment)
    {
        _db = db;
        _environment = environment;
    }

    public IActionResult Index()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
        {
            return RedirectToAction("Index", "Account");
        }

        var model = new ItemViewModel
        {
            CategorySelectListItem = _db.Categories
                .OrderBy(x => x.CategoryId)
                .Select(x => new SelectListItem
                {
                    Text = x.CategoryName,
                    Value = x.CategoryId.ToString(),
                    Selected = x.CategoryId == 1
                })
                .ToList()
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Index(ItemViewModel model)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
        {
            return RedirectToAction("Index", "Account");
        }

        if (model.ImagePath == null || model.ImagePath.Length == 0)
        {
            return Json(new { Success = false, Message = "Please choose an item image." });
        }

        var uploadsFolder = Path.Combine(_environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "images");
        Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.ImagePath.FileName)}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        await using (var stream = System.IO.File.Create(filePath))
        {
            await model.ImagePath.CopyToAsync(stream);
        }

        var item = new Item
        {
            ItemId = Guid.NewGuid(),
            CategoryId = model.CategoryId,
            Description = model.Description ?? string.Empty,
            ImagePath = "/images/" + fileName,
            ItemCode = model.ItemCode ?? string.Empty,
            ItemName = model.ItemName ?? string.Empty,
            ItemPrice = model.ItemPrice
        };

        _db.Items.Add(item);
        await _db.SaveChangesAsync();

        return Json(new { Success = true, Message = "Item is added successfully." });
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
        {
            return RedirectToAction("Index", "Account");
        }

        var item = await _db.Items.FirstOrDefaultAsync(x => x.ItemId == id);
        if (item == null)
        {
            return NotFound();
        }

        var model = new ItemViewModel
        {
            ItemId = item.ItemId,
            ItemCode = item.ItemCode,
            ItemName = item.ItemName,
            Description = item.Description,
            ItemPrice = item.ItemPrice,
            CategoryId = item.CategoryId,
            CurrentImagePath = item.ImagePath,
            CategorySelectListItem = await _db.Categories
                .OrderBy(x => x.CategoryId)
                .Select(x => new SelectListItem
                {
                    Text = x.CategoryName,
                    Value = x.CategoryId.ToString(),
                    Selected = x.CategoryId == item.CategoryId
                })
                .ToListAsync()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, ItemViewModel model)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
        {
            return RedirectToAction("Index", "Account");
        }

        var item = await _db.Items.FirstOrDefaultAsync(x => x.ItemId == id);
        if (item == null)
        {
            return NotFound();
        }

        item.CategoryId = model.CategoryId;
        item.ItemCode = model.ItemCode ?? string.Empty;
        item.ItemName = model.ItemName ?? string.Empty;
        item.Description = model.Description ?? string.Empty;
        item.ItemPrice = model.ItemPrice;

        if (model.ImagePath != null && model.ImagePath.Length > 0)
        {
            var uploadsFolder = Path.Combine(_environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "images");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.ImagePath.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            await using (var stream = System.IO.File.Create(filePath))
            {
                await model.ImagePath.CopyToAsync(stream);
            }

            item.ImagePath = "/images/" + fileName;
        }

        await _db.SaveChangesAsync();
        return RedirectToAction("Index", "Owner");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
        {
            return RedirectToAction("Index", "Account");
        }

        var item = await _db.Items.FirstOrDefaultAsync(x => x.ItemId == id);
        if (item != null)
        {
            _db.Items.Remove(item);
            await _db.SaveChangesAsync();
        }

        return RedirectToAction("Index", "Owner");
    }
}
