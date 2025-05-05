using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using v8proj.BissnessLogic.Interfaces.User; 
using v8proj.Core.Constant;
using v8proj.Core.Model.DTO.User;
using v8proj.Core.Enums.User;   
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

            var userEmailCookie = new HttpCookie("UserEmail")
            {
                Expires = DateTime.Now.AddDays(-1) 
            };
            Response.Cookies.Add(userEmailCookie);
            
            var userRoleCookie = new HttpCookie("UserRole") 
            {
                Expires = DateTime.Now.AddDays(-1) 
            };
            Response.Cookies.Add(userRoleCookie);
            

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
             
                HttpCookie userEmailCookie = new HttpCookie("UserEmail", signInDto.Email)
                {
                    Expires = DateTime.Now.AddDays(7), 
                    HttpOnly = true 
                };
                Response.Cookies.Add(userEmailCookie);

                // --- НАЧАЛО: Проверка роли и установка cookie UserRole ---
                try
                {
                    BaseResponse<UserDto> userResponse = await _userService.GetUserByEmailAsync(signInDto.Email); 
                    
                    if (userResponse != null && userResponse.Data != null) 
                    {
                        UserDto user = userResponse.Data; 
                        
                        if (user.UserType == UserType.Admin) 
                        {
                            // 5. Устанавливаем cookie для Роли
                            HttpCookie userRoleCookie = new HttpCookie("UserRole", "Admin") 
                            {
                                Expires = DateTime.Now.AddDays(7),
                                HttpOnly = true 
                            };
                            Response.Cookies.Add(userRoleCookie);
                        }
                    }
                   
                }
                catch (Exception ex)
                {
                    // Log.Error("Ошибка при получении роли пользователя для установки cookie: " + ex.Message); 
                }
// --- КОНЕЦ: Проверка роли и установка cookie UserRole ---

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