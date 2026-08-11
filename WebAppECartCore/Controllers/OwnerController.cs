using Microsoft.AspNetCore.Mvc;

namespace WebAppECartCore.Controllers;

public class OwnerController : Controller
{
    public IActionResult Index()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
        {
            return RedirectToAction("Index", "Account");
        }

        return View();
    }
}
