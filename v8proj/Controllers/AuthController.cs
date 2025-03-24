using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using v8proj.BissnessLogic.Interfaces.User;
using v8proj.Core.Constant;
using v8proj.Core.Model.DTO.User;
using v8proj.Web.Model.DTO;

namespace v8proj.Controllers
{
    public class AuthController : BaseControler
    {
        private readonly IAuthentificationSrevice _authService;

        public AuthController(IAuthentificationSrevice authService) =>
            _authService = authService;

        [HttpGet]
        public async Task<ActionResult> SignIn() => await CheckTokenValidity();
        
        [HttpGet]
        public async Task<ActionResult> SignUp() => await CheckTokenValidity();
        
        [HttpPost]
        public async Task<ActionResult> SignIn(SignInDto signInDto)
        {
            if (!ModelState.IsValid) 
                return View(signInDto);
            
            var authResponse = await _authService.SignIn(signInDto);
            
            if (!authResponse.Data)
            {
                TempData[TempDataKeys.Error] = authResponse.Message;
                return View(signInDto);
            }

            // Определяем, является ли пользователь админом
            bool isAdmin = DetermineIfAdmin(signInDto.Email);

            // Устанавливаем куки
            SetAuthCookies(signInDto.Email, isAdmin);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<ActionResult> SignUp(SignUpDto signUpDto)
        {
            if (!ModelState.IsValid) 
                return View(signUpDto);

            var authResponse = await _authService.SignUp(signUpDto);
            
            if (!authResponse.Data)
            {
                TempData[TempDataKeys.Error] = authResponse.Message;
                return View(signUpDto);
            }

            // Новые пользователи по умолчанию не админы
            SetAuthCookies(signUpDto.Email, isAdmin: false);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<ActionResult> SignOut()
        {
            ClearAuthCookies();
            return RedirectToAction("SignIn", "Auth");
        }

        private bool DetermineIfAdmin(string email)
        {
            // Ваша логика определения админа
            // Пример: админы имеют определенный email или домен
            return email.EndsWith("@admin.com") || 
                   email.Equals("admin@example.com", StringComparison.OrdinalIgnoreCase);
        }

        private void SetAuthCookies(string email, bool isAdmin)
        {
            // Кука с email
            Response.Cookies.Add(new HttpCookie("UserEmail", email)
            {
                Expires = DateTime.Now.AddDays(7),
                HttpOnly = true,
                Path = "/"
            });

            // Кука с ролью
            Response.Cookies.Add(new HttpCookie("UserRole", isAdmin ? "Admin" : "User")
            {
                Expires = DateTime.Now.AddDays(7),
                HttpOnly = true,
                Path = "/"
            });
        }

        private void ClearAuthCookies()
        {
            var cookiesToClear = new[] { "UserEmail", "UserRole" };
            foreach (var cookieName in cookiesToClear)
            {
                Response.Cookies.Add(new HttpCookie(cookieName)
                {
                    Expires = DateTime.Now.AddDays(-1),
                    Path = "/"
                });
            }
        }

        private async Task<ActionResult> CheckTokenValidity()
        {
            var response = await _authService.IsTokenValid();
            if (response.Data)
                return Redirect("/Home/Index");
            return View();
        }
    }
}