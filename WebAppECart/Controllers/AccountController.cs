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
                var userDetails = db.logins.Where(x => x.userName == userAccount.UserName && x.userPassword == userAccount.UserPassword).FirstOrDefault();
                if (userDetails == null)
                {
                    userAccount.LoginErrorMessage = "Wrong user name or password.";
                    return View("Index", userAccount);
                }
                else
                {
                    Session["UserId"] = userAccount.UserId;
                    Session["UserName"] = userAccount.UserName;
                    return RedirectToAction("Index", "Owner", new { });
                }
            }
        }

        public ActionResult LogOut()
        {
            int userId = (int)Session["UserId"];
            Session.Abandon();
            return RedirectToAction("Index", "Home", new { });

        }

    }
}
