using System;
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
using v8proj.BissnessLogic.Interfaces.Posts;
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
        private readonly IPostService _postService;

        public AdminController(IUserService userService, IHomeService homeService, IReportService reportService,
            IPostService postService) : base(homeService)
        {
            _userService = userService;
            _reportService = reportService;
            _postService = postService;
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
                ViewBag.ErrorMessage = response.Message ?? "Failed to load users.";
            }
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BanUser(int userId)
        {
            int currentAdminId = await GetCurrentUserIdAsync();
            if (userId == currentAdminId)
            {
                TempData["Error"] = "You cannot ban yourself.";
                return RedirectToAction("Users");
            }

            var response = await _userService.BanUserAsync(userId);
            if (response.Status == OperationStatus.Success)
            {
                TempData["Success"] = "User successfully banned!";
            }
            else
            {
                TempData["Error"] = response.Message ?? "Failed to ban user.";
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
                TempData["Success"] = "User successfully unbanned!";
            }
            else
            {
                TempData["Error"] = response.Message ?? "Failed to unban user.";
            }
            return RedirectToAction("Users");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MakeAdmin(int userId)
        {
            int currentAdminId = await GetCurrentUserIdAsync();
            if (userId == currentAdminId)
            {
                TempData["Error"] = "You are already an administrator.";
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
                TempData["Error"] = "You cannot revoke your own administrator rights.";
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
                TempData["ErrorMessage"] = "Please enter resolution details.";
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeletePost(int id)
        {
            System.Diagnostics.Debug.WriteLine($"AdminController.DeletePost: Request to delete PostId: {id}");

            try
            {
                var response = await _postService.DeletePost(id);

                System.Diagnostics.Debug.WriteLine($"AdminController.DeletePost: PostService Response - Success: {response.Status == OperationStatus.Success}, Message: {response.Message}");

                if (response.Status == OperationStatus.Success)
                {
                    return Json(new { success = true, message = response.Message });
                }
                else
                {
                    return Json(new { success = false, message = response.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An unhandled error occurred on the server while deleting the post." });
            }
        }
    }
}