using System.Web.Mvc;

namespace WebAppECart.Controllers
{

    public class HomeController : Controller
    {

        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult About()
        {
            return View();
        }
        public ActionResult Contact()
        {
            return View();
        }
        public ActionResult Loogin()
        {
            return View();
        }

        public ActionResult ShoppingCart()
        {
            return RedirectToAction("ShoppingCart", "Shopping", new { });
        }

    }
}