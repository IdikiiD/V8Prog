using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using v8proj.Core.Entities;
using v8proj.Core.Model; // Вероятно, это Model.DTO.User
using v8proj.Web.Model.ViewModels;
using v8proj.BissnessLogic.Interfaces.Posts; // Добавьте эту строку для IPostService
using v8proj.BissnessLogic.Interfaces.User; // Добавьте эту строку для IUserService

namespace v8proj.Controllers
{
    public class PostController : Controller
    {
        // УДАЛИЛИ: private readonly ApplicationDbContext _context = new ApplicationDbContext();

        private readonly IPostService _postService; // Теперь мы инжектируем IPostService
        private readonly IUserService _userService; // Инжектируем IUserService, так как он используется

        // НОВЫЙ КОНСТРУКТОР: Unity будет использовать его для создания PostController
        public PostController(IPostService postService, IUserService userService)
        {
            _postService = postService;
            _userService = userService;
        }

        // GET: Post/Create
        public ActionResult Create()
        {
            ViewBag.Categories = new List<string> { "Sedan", "SUV", "Pickup", "Convertible" };
            return View();
        }

        // GET: Post/Details/5
        public ActionResult Details(int id)
        {
            // Используем _postService для получения поста
            var post = _postService.GetPostById(id);
            if (post == null) return HttpNotFound();

            // Логика работы с файлами (не связана с DbContext, остается здесь или в отдельном сервисе)
            if (!string.IsNullOrEmpty(post.ImagePath1))
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content/Uploads/Posts",
                    Path.GetFileName(post.ImagePath1));
                if (System.IO.File.Exists(path))
                {
                    ViewBag.Image1Base64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(path));
                }
            }

            if (!string.IsNullOrEmpty(post.ImagePath2))
            {
                var path = Server.MapPath("~" + post.ImagePath2);
                if (System.IO.File.Exists(path))
                {
                    ViewBag.Image2Base64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(path));
                }
            }

            if (!string.IsNullOrEmpty(post.ImagePath3))
            {
                var path = Server.MapPath("~" + post.ImagePath3);
                if (System.IO.File.Exists(path))
                {
                    ViewBag.Image3Base64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(path));
                }
            }
            return View(post);
        }

        // POST: Post/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PostCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new List<string> { "Sedan", "SUV", "Pickup", "Convertible" };
                return View(model);
            }

            try
            {
                var post = new Post
                {
                    Title = model.Title,
                    Description = model.Description,
                    Category = model.Category,
                    ImagePath1 = ProcessUploadedFile(model.Image1, "Image1"),
                    ImagePath2 = ProcessUploadedFile(model.Image2, "Image2"),
                    ImagePath3 = ProcessUploadedFile(model.Image3, "Image3"),
                    CreatedAt = DateTime.Now
                };

                if (post.ImagePath1 == null && post.ImagePath2 == null && post.ImagePath3 == null)
                {
                    ModelState.AddModelError("", "Необходимо загрузить хотя бы одно изображение.");
                    ViewBag.Categories = new List<string> { "Sedan", "SUV", "Pickup", "Convertible" };
                    return View(model);
                }

                _postService.AddPost(post);      // Используем сервис для добавления поста
                _postService.SaveChanges();      // Используем сервис для сохранения изменений

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Ошибка при создании поста: {ex.Message}");
                ModelState.AddModelError("", "Произошла ошибка при сохранении данных.");
                ViewBag.Categories = new List<string> { "Sedan", "SUV", "Pickup", "Convertible" };
                return View(model);
            }
        }

        // Методы ProcessUploadedFile, IsValidImage, Image остаются без изменений,
        // так как они не работают напрямую с DbContext.
        private string ProcessUploadedFile(HttpPostedFileBase file, string fieldName)
        {
            // ... (оставьте этот код без изменений)
            if (file == null || file.ContentLength == 0)
                return null;

            if (file.ContentLength > 5 * 1024 * 1024)
            {
                ModelState.AddModelError(fieldName, "Размер файла не должен превышать 5MB.");
                return null;
            }

            if (!IsValidImage(file))
            {
                ModelState.AddModelError(fieldName, "Допустимы только файлы изображений (JPG, JPEG, PNG, GIF).");
                return null;
            }

            try
            {
                var uploadsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content/Uploads/Posts");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                file.SaveAs(filePath);

                return $"/Content/Uploads/Posts/{uniqueFileName}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Ошибка при сохранении файла ({fieldName}): {ex.Message}");
                ModelState.AddModelError(fieldName, "Ошибка при сохранении файла. Убедитесь, что путь существует.");
                return null;
            }
        }

        private bool IsValidImage(HttpPostedFileBase file)
        {
            // ... (оставьте этот код без изменений)
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            return allowedExtensions.Contains(fileExtension);
        }

        // МЕТОД DISPOSE УДАЛЕН. Unity управляет жизненным циклом DbContext.
        // protected override void Dispose(bool disposing) { ... }

        public ActionResult Image(string file)
        {
            // ... (оставьте этот код без изменений)
            if (string.IsNullOrEmpty(file))
                return HttpNotFound();

            var path = Server.MapPath("~/Content/Uploads/Posts/" + file);

            if (!System.IO.File.Exists(path))
                return HttpNotFound();

            var mimeType = MimeMapping.GetMimeMapping(path);
            return File(path, mimeType);
        }

        // Отображение избранных постов
        public ActionResult Favorites()
        {
            var userEmail = Request.Cookies["UserEmail"]?.Value;
            if (string.IsNullOrEmpty(userEmail))
                return RedirectToAction("SignIn", "Auth");

            // Используем _postService для получения пользователя с избранным
            var user = _postService.GetUserWithFavorites(userEmail);

            if (user == null)
                return HttpNotFound();

            var favorites = user.FavoritePosts.ToList();
            return View(favorites);
        }

        // Метод для добавления/удаления поста из избранного
        [HttpPost]
        public ActionResult ToggleFavorite(int id)
        {
            var userEmailCookie = Request.Cookies["UserEmail"];
            if (userEmailCookie == null)
                return new HttpStatusCodeResult(401, "Unauthorized");

            var userEmail = userEmailCookie.Value;

            // Используем _postService для получения пользователя с избранным
            var user = _postService.GetUserWithFavorites(userEmail);

            if (user == null)
                return new HttpStatusCodeResult(401, "Unauthorized");

            var post = _postService.GetPostWithFavorites(id); // Используем сервис
            if (post == null)
                return HttpNotFound();

            if (post.FavoritedBy.Any(u => u.UserId == user.UserId))
            {
                post.FavoritedBy.Remove(user);
            }
            else
            {
                post.FavoritedBy.Add(user);
            }

            _postService.SaveChanges(); // Используем сервис

            return Json(new { success = true });
        }
    }
}