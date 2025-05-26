using System.Threading.Tasks;
using System.Web.Mvc;
using v8proj.BissnessLogic.Interfaces.User;
using v8proj.Core.Enums;
using v8proj.Web.Model.ViewModels;
using v8proj.Web.ViewModels;

namespace v8proj.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IUserService _userService;

        public ProfileController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var email = User.Identity.Name;
                var response = await _userService.GetUserByEmailAsync(email);

                if (response.Data != null)
                {
                    var user = response.Data;
                    var updateResponse = await _userService.UpdatePhoneNumberAsync(user.Id, model.NewPhoneNumber); // Используем user.Id

                    if (updateResponse.Status == OperationStatus.Success)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Не удалось добавить номер телефона.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Пользователь не найден.");
                }
            }
            return View("Index", model);
        }

        public async Task<ActionResult> Index()
        {
            var email = User.Identity.Name;
            var response = await _userService.GetUserByEmailAsync(email);

            if (response.Data == null)
            {
                return View("Error");
            }

            var user = response.Data;

            // Временный вывод для проверки
            System.Diagnostics.Debug.WriteLine($"Email: {user.Email}, FullName: {user.FullName}, RegistrationDate: {user.DateRegistered}, PhoneNumber: {user.PhoneNumber}");

            var viewModel = new ProfileViewModel
            {
                Email = user.Email,
                FullName = user.FullName,
                AvatarUrl = user.AvatarUrl,
                PhoneNumber = user.PhoneNumber,
                RegistrationDate = user.DateRegistered
            };

            return View("Index", viewModel);
        }
    }
}