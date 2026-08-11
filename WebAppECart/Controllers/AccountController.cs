using System.Linq;
using System.Web.Mvc;
using WebAppECart.Models;

namespace WebAppECart.Controllers
{
    public class AccountController : Controller
    {
        // GET: /Login/
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Authorization(WebAppECart.ViewModel.AccountViewModel userAccount)
        {
            using (AlohaTableRestaurantDBEntities db = new AlohaTableRestaurantDBEntities())
            {
                var userDetails = db.logins
                    .Where(x => x.userName == userAccount.UserName && x.userPassword == userAccount.UserPassword)
                    .FirstOrDefault();

                if (userDetails == null)
                {
                    userAccount.LoginErrorMessage = "Wrong user name or password.";
                    return View("Index", userAccount);
                }

                Session["UserId"] = userDetails.userId;
                Session["UserName"] = userDetails.userName;
                return RedirectToAction("Index", "Owner");
            }
        }

        public ActionResult LogOut()
        {
            if (Session != null && Session["UserId"] != null)
            {
                Session.Abandon();
            }

            return RedirectToAction("Index", "Home");
        }

    }
}
