using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using v8proj.BissnessLogic.Interfaces.User;
using v8proj.Core.Enums.User;
using v8proj.Core.Model.DTO.User;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using v8proj.Core.Enums; // <----  ДОБАВЬТЕ ЭТУ СТРОКУ

namespace v8proj.Controllers
{
    public class AdminController : HomeController
    {
        private readonly IUserService _userService;
        //private readonly UserManager<ApplicationUser> _userManager; // Удалите, если не используете ASP.NET Identity

        //public AdminController(IUserService userService, UserManager<ApplicationUser> userManager) // Измените конструктор
        public AdminController(IUserService userService)
        {
            _userService = userService;
            // _userManager = userManager;
        }

        private bool IsCurrentUserAdmin()
        {
            //return User.IsInRole("Admin"); // Используйте это, если у вас ASP.NET Identity с ролями
            ClaimsIdentity identity = (ClaimsIdentity)User.Identity;
            return identity.HasClaim(ClaimTypes.Role, "Admin");
        }

        private int GetCurrentUserId()
        {
            // return int.Parse(User.Identity.GetUserId()); //Если используете ASP.NET Identity
            // Замените на вашу логику получения ID пользователя.
            Claim idClaim = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim != null && int.TryParse(idClaim.Value, out int userId))
            {
                return userId;
            }
            return -1;
        }

        public ActionResult Dashboard()
        {
            return View();
        }

        public async Task<ActionResult> Users(string searchString = null, int page = 1, int pageSize = 10)
        {
            ViewBag.CurrentSearch = searchString;

            var response = await _userService.GetUsersAsync(searchString, UserType.None, page, pageSize);

            List<UserDto> users = new List<UserDto>();
            if (response.Data != null)
            {
                users = response.Data;
            }
            else
            {
                ViewBag.ErrorMessage = response.Message ?? "Failed to upload users.";
            }

            //ViewBag.IsAdmin = IsCurrentUserAdmin(); //не нужно передавать это в view.
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BanUser(int userId)
        {
            var userToBan = await _userService.GetUserByIdAsync(userId);
            if (userToBan.Data != null && userToBan.Data.UserType == UserType.Admin)
            {
                TempData["Error"] = "Администраторы не могут банить других администраторов.";
                return RedirectToAction("Users");
            }

            var response = await _userService.BanUserAsync(userId);
            if (response.Status == OperationStatus.Success) // Исправлено сравнение
            {
                TempData["Success"] = "Пользователь успешно забанен!.";
            }
            else
            {
                TempData["Error"] = response.Message ?? "Не удалось забанить пользователя.";
            }
            return RedirectToAction("Users");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UnbanUser(int userId)
        {
            var response = await _userService.UnbanUserAsync(userId);
            if (response.Status == OperationStatus.Success) // Исправлено сравнение
            {
                TempData["Success"] = "Пользователь успешно разбанен!.";
            }
            else
            {
                TempData["Error"] = response.Message ?? "Не удалось разбанить пользователя.";
            }
            return RedirectToAction("Users");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MakeAdmin(int userId)
        {
            var userToMakeAdmin = await _userService.GetUserByIdAsync(userId);
            if (userToMakeAdmin.Data != null && userToMakeAdmin.Data.UserType == UserType.Admin)
            {
                TempData["Error"] = "Нельзя сделать админа, другим админом";
                return RedirectToAction("Users");
            }

            var response = await _userService.MakeUserAdminAsync(userId); // Исправлен вызов метода
            if (response.Status == OperationStatus.Success) // Исправлено сравнение
            {
                TempData["Success"] = response.Message;
            }
            else
            {
                TempData["Error"] = response.Message;
            }
            return RedirectToAction("Users");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RevokeAdmin(int userId)
        {
            if (GetCurrentUserId() == userId)
            {
                TempData["Error"] = "Вы не можете лишить себя прав администратора.";
                return RedirectToAction("Users");
            }
            var userToRevoke = await _userService.GetUserByIdAsync(userId);
            if (userToRevoke.Data != null && userToRevoke.Data.UserType != UserType.Admin)
            {
                TempData["Error"] = "Можно забрать права только у админа";
                return RedirectToAction("Users");
            }

            var response = await _userService.RevokeUserAdminAsync(userId); // Исправлен вызов метода
            if (response.Status == OperationStatus.Success) // Исправлено сравнение
            {
                TempData["Success"] = response.Message;
            }
            else
            {
                TempData["Error"] = response.Message;
            }
            return RedirectToAction("Users");
        }

        public ActionResult Concepts()
        {
            return View();
        }

        public ActionResult Settings()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveSettings(string siteName, int maxUploadSize, string theme)
        {
            ViewBag.Message = "Settings saved successfully!";
            return View("Settings");
        }
    }
}