using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantWebApp.Models;

namespace RestaurantWebApp.Controllers;

public class AccountController : Controller
{
    private readonly AlohaTableDbContext _db;

    public AccountController(AlohaTableDbContext db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Authorization(AccountViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        var user = await _db.Logins
            .FirstOrDefaultAsync(x => x.UserName == model.UserName && x.UserPassword == model.UserPassword);

        if (user == null)
        {
            model.LoginErrorMessage = "Wrong user name or password.";
            return View("Index", model);
        }

        HttpContext.Session.SetString("UserName", user.UserName);
        HttpContext.Session.SetString("UserId", user.UserId.ToString());
        return RedirectToAction("Index", "Owner");
    }

    public IActionResult LogOut()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}
