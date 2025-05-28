using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web; 
using v8proj.BissnessLogic.Interfaces.User;
using v8proj.Core.Enums.User;
using v8proj.Core.Model.DTO.User;
using System.Linq;
using v8proj.Core.Enums;
using v8proj.BissnessLogic.Interfaces.Home;
using v8proj.BissnessLogic.Interfaces.Reports;
using v8proj.BissnessLogic.Services.Home;
using v8proj.Core.Model.DTO.Report;
using v8proj.Web.Filters;

namespace v8proj.Controllers
{
    [Auth(Roles = "Admin")] 
    public class AdminController : HomeController
    {
        private readonly IUserService _userService;
        private readonly IReportService _reportService;

        public AdminController(IUserService userService, IHomeService homeService, IReportService reportService) : base(homeService)
        {
            _userService = userService;
            _reportService = reportService;
        }

        private async Task<int> GetCurrentUserIdAsync()
        {
            HttpCookie userEmailCookie = Request.Cookies["UserEmail"];
            if (userEmailCookie == null || string.IsNullOrEmpty(userEmailCookie.Value))
            {
                return -1;
            }

            string userEmail = userEmailCookie.Value;
            var userResponse = await _userService.GetUserByEmailAsync(userEmail);
            if (userResponse.Data != null)
            {
                return userResponse.Data.Id;
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
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BanUser(int userId)
        {
            var userToBan = await _userService.GetUserByIdAsync(userId);
            int currentAdminId = await GetCurrentUserIdAsync();
            if (userToBan.Data != null && userToBan.Data.Id == currentAdminId)
            {
                TempData["Error"] = "Вы не можете забанить самого себя.";
                return RedirectToAction("Users");
            }

            if (userToBan.Data != null && userToBan.Data.UserType == UserType.Admin)
            {
                TempData["Error"] = "Администраторы не могут банить других администраторов.";
                return RedirectToAction("Users");
            }

            var response = await _userService.BanUserAsync(userId);
            if (response.Status == OperationStatus.Success)
            {
                TempData["Success"] = "Пользователь успешно забанен!"; 
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
            if (response.Status == OperationStatus.Success)
            {
                TempData["Success"] = "Пользователь успешно разбанен!"; 
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
            int currentAdminId = await GetCurrentUserIdAsync();
            if (userToMakeAdmin.Data != null && userToMakeAdmin.Data.Id == currentAdminId)
            {
                TempData["Error"] = "Вы уже администратор.";
                return RedirectToAction("Users");
            }

            if (userToMakeAdmin.Data != null && userToMakeAdmin.Data.UserType == UserType.Admin)
            {
                TempData["Error"] = "Нельзя сделать админа, другим админом"; 
                return RedirectToAction("Users");
            }

            var response = await _userService.MakeUserAdminAsync(userId);
            if (response.Status == OperationStatus.Success)
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
            int currentAdminId = await GetCurrentUserIdAsync();
            if (currentAdminId == userId)
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

            var response = await _userService.RevokeUserAdminAsync(userId);
            if (response.Status == OperationStatus.Success)
            {
                TempData["Success"] = response.Message;
            }
            else
            {
                TempData["Error"] = response.Message;
            }
            return RedirectToAction("Users");
        }

        public async Task<ActionResult> Concepts()
        {
            var unresolvedReports = await _reportService.GetUnresolvedReports();
            return View(unresolvedReports);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResolveReport(int reportId, string resolutionDetails)
        {
            if (string.IsNullOrEmpty(resolutionDetails))
            {
                TempData["ErrorMessage"] = "Пожалуйста, введите детали решения.";
                return RedirectToAction("Concepts");
            }

            var response = await _reportService.ResolveReport(reportId, resolutionDetails);

            if (response.Status == OperationStatus.Success)
            {
                TempData["SuccessMessage"] = response.Message;
            }
            else
            {
                TempData["ErrorMessage"] = response.Message;
            }

            return RedirectToAction("Concepts");
        }

        public async Task<ActionResult> ReportDetails(int id)
        {
            var report = await _reportService.GetReportById(id);
            if (report == null)
            {
                return HttpNotFound();
            }

            return View(report);
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