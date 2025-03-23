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
        public async Task<ActionResult> SignIn(SignInDto signInDto) =>
            await ProcessAuthentication(async () => await _authService.SignIn(signInDto), signInDto);

        [HttpPost]
        public async Task<ActionResult> SignUp(SignUpDto signUpDto) =>
            await ProcessAuthentication(async () => await _authService.SignUp(signUpDto), signUpDto);

        [HttpGet]
        public async Task<ActionResult> SignOut()
        {
            // Удаляем cookies
            var userCookie = new HttpCookie("UserEmail")
            {
                Expires = DateTime.Now.AddDays(-1) // Устанавливаем дату истечения в прошлое, чтобы удалить cookie
            };
            Response.Cookies.Add(userCookie);

            // Перенаправляем на страницу входа
            return RedirectToAction("SignIn", "Auth");
        }

        private async Task<ActionResult> ProcessAuthentication
            (Func<Task<BaseResponse<bool>>> authServiceMethod, object dto)
        {
            
            if (!ModelState.IsValid) return View(dto is SignInDto ? "SignIn" : "SignUp", dto);
            
            var authResponse = await authServiceMethod();

            if (!authResponse.Data)
            {
                TempData[TempDataKeys.Error] = authResponse.Message;
                return View(dto is SignInDto ? "SignIn" : "SignUp", dto);

            }

            if (dto is SignInDto signInDto)
            {
                HttpCookie userCookie = new HttpCookie("UserEmail", signInDto.Email)
                {
                    Expires = DateTime.Now.AddDays(7), // Храним 7 дней
                    HttpOnly = true // Защищаем от доступа через JS
                };
                Response.Cookies.Add(userCookie);
            }

            return RedirectToAction("Index", "Home");
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
