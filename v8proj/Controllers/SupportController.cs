using System.Web.Mvc;
using System.Linq;
using v8proj.Core.Interface.Support;
using v8proj.DAL;

namespace v8proj.Controllers
{
    public class SupportController : Controller
    {
        private readonly ISupportRepository _supportRepository;
        private readonly ApplicationDbContext _dbContext;

        public SupportController(ISupportRepository supportRepository, ApplicationDbContext dbContext)
        {
            _supportRepository = supportRepository;
            _dbContext = dbContext;
        }

        [HttpPost]
        public ActionResult Support(int PostId, decimal Amount, string Comment, string CardNumber, string ExpiryMonth, string ExpiryYear, string CVV)
        {
            // Получаем email текущего пользователя из куки
            var userEmailCookie = Request.Cookies["UserEmail"];
            v8proj.Core.Entities.User.UserEf currentUser = null;
            if (userEmailCookie != null)
            {
                currentUser = _dbContext.Users.FirstOrDefault(u => u.Email == userEmailCookie.Value);
            }

            var support = new v8proj.Core.Entities.Support
            {
                PostId = PostId,
                Amount = Amount,
                Comment = Comment,
                CardNumber = CardNumber,
                ExpiryMonth = ExpiryMonth,
                ExpiryYear = ExpiryYear,
                CVV = CVV,
                UserId = currentUser?.UserId
            };

            _supportRepository.Add(support);
            _supportRepository.SaveChanges();

            return RedirectToAction("ThankYou");
        }

        public ActionResult ThankYou()
        {
            return View();
        }
    }
}