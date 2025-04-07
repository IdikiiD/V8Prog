using System.Web.Mvc;

namespace v8proj.Controllers
{
    public class AdminController : Controller
    {
        // Главная страница админки
        public ActionResult Dashboard()
        {
            return View("~/Views/Admin/Dashboard.cshtml");
        }

        // Управление пользователями
        public ActionResult Users()
        {
            return View("~/Views/Admin/User.cshtml");
        }

        // Управление концепт-артами
        public ActionResult Concepts()
        {
            return View("~/Views/Admin/Concepts.cshtml");
        }

        // Настройки
        public ActionResult Settings()
        {
            return View("~/Views/Admin/Settings.cshtml");
        }
    }
}