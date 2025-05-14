using System.Linq;
using System.Web.Mvc;
using v8proj.DAL;
using v8proj.Web.Model;
using v8proj.Web.Model.ViewModels;
using v8proj.Core.Model; 


namespace v8proj.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext(); // Подключение к базе

        public ActionResult Index(string category)
        {
            var cars = _context.eUseControl.ToList();

            var postsQuery = _context.Posts.AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                postsQuery = postsQuery.Where(p => p.Category == category);
            }

            var posts = postsQuery
                .Where(p => !string.IsNullOrEmpty(p.ImagePath1))
                .OrderByDescending(p => p.CreatedAt) // ← это ключевой момент
                .ToList();


            foreach (var post in posts)
            {
                if (string.IsNullOrEmpty(post.ImagePath1))
                {
                    post.ImagePath1 = "/Content/Uploads/Posts/no-image.png"; // Заглушка
                }
            }

            var viewModel = new HomeViewModel
            {
                Cars = cars,
                Posts = posts
            };

            return View(viewModel);
        }


        public ActionResult Cart() => View();
        public ActionResult SignUp() => View("~/Views/Auth/SignUp.cshtml");
        public ActionResult SignIn() => View("~/Views/Auth/SignIn.cshtml");
        public ActionResult Setings() => View();
        public ActionResult ForgotPassword() => View();
        public ActionResult Profile() => View("~/Views/Profile/Profile.cshtml");
        public ActionResult ProductManagment() => View("~/Views/Admin/ProductManagment.cshtml");
    }
}