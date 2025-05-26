using System.Linq;
using System.Web.Mvc;
using v8proj.Web.Model;
using v8proj.Web.Model.ViewModels;
using v8proj.Core.Entities; 
using v8proj.BissnessLogic.Interfaces.Home;
using v8proj.BissnessLogic.Services.Home;

namespace v8proj.Controllers
{
    public class HomeController : Controller
    {
        

        private readonly IHomeService _homeService; 

        
        public HomeController(IHomeService homeService)
        {
            _homeService = homeService;
        }

        public ActionResult Index(string category)
        {
        
            var cars = _homeService.GetCars();
            var posts = _homeService.GetPosts(category);

            foreach (var post in posts)
            {
                if (string.IsNullOrEmpty(post.ImagePath1))
                {
                    post.ImagePath1 = "/Content/Uploads/Posts/no-image.png"; 
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