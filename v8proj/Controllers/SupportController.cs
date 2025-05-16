using System.Web.Mvc;

namespace v8proj.Controllers
{
    public class SupportController : Controller
    {
        [HttpPost]
        public ActionResult Support(SupportModel model)
        {
            // Логика обработки данных
            // Например, сохранение транзакции или других данных

            // После успешной обработки перенаправление на страницу благодарности
            return RedirectToAction("ThankYou");
        }

        public ActionResult ThankYou()
        {
            // Представление благодарности
            return View();
        }
    }
}