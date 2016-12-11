using System.Web.Mvc;

namespace Concurrency.Web.MVCControllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Authors()
        {
            return View();
        }

        public ActionResult Books()
        {
            return View();
        }
    }
}