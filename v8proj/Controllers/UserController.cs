using System.Threading.Tasks;
using System.Web.Mvc;
using v8proj.BissnessLogic.Interfaces.User;
using v8proj.Web.ViewModels;

public class UserController : Controller
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<ActionResult> Profile()
    {
        var emailCookie = Request.Cookies["UserEmail"];
        if (emailCookie == null)
        {
            return RedirectToAction("SignIn", "Auth");  // Если нет email в cookies, редирект на страницу входа
        }

        var email = emailCookie.Value;

        // Получаем данные пользователя по email
        var userResponse = await _userService.GetUserByEmailAsync(email);

        if (userResponse.Data == null)
        {
            return View("Error");  // Если данные не найдены
        }

        var user = userResponse.Data;

        // Создаем ViewModel для отображения данных на странице
        var viewModel = new ProfileViewModel
        {
            Email = user.Email,
            FullName = user.FullName,
            AvatarUrl = user.AvatarUrl
        };

        return View(viewModel);
    }

}
