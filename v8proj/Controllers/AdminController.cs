
using System.Collections.Generic; 
using System.Threading.Tasks; 
using System.Web.Mvc;
using v8proj.BissnessLogic.Interfaces.User; 
using v8proj.Core.Enums.User;         
using v8proj.Core.Model.DTO.User;    

namespace v8proj.Controllers
{
    
  
    public class AdminController : HomeController
    {
    
        private readonly IUserService _userService;

    
        public AdminController(IUserService userService)
        {
            _userService = userService;
        }
     

        public ActionResult Dashboard()
        {
            
            return View();
        }
        
        [HttpGet] 
        public async Task<ActionResult> Users(int page = 1, int pageSize = 10, string searchEmail = null)
        {
            
            var response = await _userService.GetUsersAsync(searchEmail, UserType.None, page, pageSize); 

            List<UserDto> users = new List<UserDto>();
            if (response.Data != null)
            {
                users = response.Data;
            }
            
            
            return View(users);
        }
   
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BanUser(int userId)
        {
            var response = await _userService.BanUserAsync(userId);

            if (!response.Data)
            {
                TempData["Error"] = response.Message ?? "Could not ban user.";
            }
            else
            {
                TempData["Success"] = "User successfully banned!.";
            }

            
            return RedirectToAction("Users"); 
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UnbanUser(int userId)
        {
            var response = await _userService.UnbanUserAsync(userId); 

            if (!response.Data) 
            {
                TempData["Error"] = response.Message ?? "Could not unban user.";
            }
            else
            {
                TempData["Success"] = "User successfully unbanned!.";
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