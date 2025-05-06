using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using v8proj.BissnessLogic.Interfaces.User;
using v8proj.Core.Constant;
using v8proj.Core.Model.DTO.User;
using v8proj.Core.Enums.User;
using v8proj.Core.Enums.Entinity; 
using v8proj.Web.Model.DTO;


namespace v8proj.Controllers
{
    public class AuthController : BaseControler
    {
        private readonly IAuthentificationSrevice _authService;
        private readonly IUserService _userService;

        public AuthController(IAuthentificationSrevice authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

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
            ClearAuthCookies(); 
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
              
                BaseResponse<UserDto> userResponse = await _userService.GetUserByEmailAsync(signInDto.Email);

                if (userResponse.Data == null) 
                {
                   
                    ModelState.AddModelError("", "Ошибка при получении данных пользователя.");
                    return View("SignIn", signInDto);
                }

                UserDto user = userResponse.Data;

                // --- НАЧАЛО: ПРОВЕРКА СТАТУСА ПОЛЬЗОВАТЕЛЯ (НА БАН) ---
                if (user.UserStatus == EntityStatus.Banned)
                {
                    ModelState.AddModelError("", "Your account is blocked");
                 
                    ClearAuthCookies();
                    return View("SignIn", signInDto); 
                }
                // --- КОНЕЦ: ПРОВЕРКА СТАТУСА ПОЛЬЗОВАТЕЛЯ ---

           
                HttpCookie userEmailCookie = new HttpCookie("UserEmail", signInDto.Email)
                {
                    Expires = DateTime.Now.AddDays(7),
                    HttpOnly = true
                };
                Response.Cookies.Add(userEmailCookie);

               
                if (user.UserType == UserType.Admin)
                {
                    HttpCookie userRoleCookie = new HttpCookie("UserRole", "Admin")
                    {
                        Expires = DateTime.Now.AddDays(7),
                        HttpOnly = true
                    };
                    Response.Cookies.Add(userRoleCookie);
                }

                return RedirectToAction("Index", "Home"); 
            }
            else if (dto is SignUpDto signUpDtoInstance) 
            {
                
                BaseResponse<UserDto> userResponse = await _userService.GetUserByEmailAsync(signUpDtoInstance.Email);
                if (userResponse.Data != null)
                {
                    UserDto user = userResponse.Data;
                }
                return RedirectToAction("SignIn", "Auth"); 
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

     
        private void ClearAuthCookies()
        {
            
            if (Request.Cookies["UserEmail"] != null)
            {
                var userEmailCookie = new HttpCookie("UserEmail")
                {
                    Expires = DateTime.Now.AddDays(-1)
                };
                Response.Cookies.Set(userEmailCookie); 
            }

            // Удаляем UserRole cookie
            if (Request.Cookies["UserRole"] != null)
            {
                var userRoleCookie = new HttpCookie("UserRole")
                {
                    Expires = DateTime.Now.AddDays(-1)
                };
                Response.Cookies.Set(userRoleCookie);
            }

        }
    }
}