using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Azure.Core;
using v8proj.Core.Entities;
using v8proj.DAL;
using v8proj.Core.Model;
using v8proj.Web.Model.ViewModels;
using System.Data.Entity;


namespace v8proj.Controllers
{
    public class PostController : Controller
    {


        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        // GET: Post/Create
        public ActionResult Create()
        {
            ViewBag.Categories = new List<string> { "Sedan", "SUV", "Pickup", "Convertible" };
            return View();
        }

        // GET: Post/Details/5
        public ActionResult Details(int id)

        {
            var post = _context.Posts.Find(id);
            if (post == null) return HttpNotFound();

            // Отображение изображений как Base64
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
                    CreatedAt = DateTime.Now // Устанавливаем дату и время создания
                };

                // Проверка, что хотя бы одно изображение загружено
                if (post.ImagePath1 == null && post.ImagePath2 == null && post.ImagePath3 == null)
                {
                    ModelState.AddModelError("", "Необходимо загрузить хотя бы одно изображение.");
                    ViewBag.Categories = new List<string> { "Sedan", "SUV", "Pickup", "Convertible" };
                    return View(model);
                }

                _context.Posts.Add(post);
                _context.SaveChanges();

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                System.Diagnostics.Trace.TraceError($"Ошибка при создании поста: {ex.Message}");
                ModelState.AddModelError("", "Произошла ошибка при сохранении данных.");
                ViewBag.Categories = new List<string> { "Sedan", "SUV", "Pickup", "Convertible" };
                return View(model);
            }
        }


        private string ProcessUploadedFile(HttpPostedFileBase file, string fieldName)
        {
            if (file == null || file.ContentLength == 0)
                return null;

            // Проверка размера
            if (file.ContentLength > 5 * 1024 * 1024)
            {
                ModelState.AddModelError(fieldName, "Размер файла не должен превышать 5MB.");
                return null;
            }

            // Проверка расширения
            if (!IsValidImage(file))
            {
                ModelState.AddModelError(fieldName, "Допустимы только файлы изображений (JPG, JPEG, PNG, GIF).");
                return null;
            }


            try
            {
                // Путь до папки Content/Uploads/Posts
                var uploadsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content/Uploads/Posts");

                // Создать папку при необходимости
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                file.SaveAs(filePath);

                // Возвращаем относительный путь для использования в <img src="...">
                return $"/Content/Uploads/Posts/{uniqueFileName}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Ошибка при сохранении файла ({fieldName}): {ex.Message}");
                ModelState.AddModelError(fieldName, "Ошибка при сохранении файла. Убедитесь, что путь существует.");
                return null;
            }
        }



        // Проверка расширения файла
        private bool IsValidImage(HttpPostedFileBase file)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            return allowedExtensions.Contains(fileExtension);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }

            base.Dispose(disposing);
        }

        public ActionResult Image(string file)
        {
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

            var user = _context.Users
                .Include(u => u.FavoritePosts)
                .FirstOrDefault(u => u.Email == userEmail);

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
            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
            if (user == null)
                return new HttpStatusCodeResult(401, "Unauthorized");

            var post = _context.Posts.Include(p => p.FavoritedBy).FirstOrDefault(p => p.Id == id);
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

            _context.SaveChanges();

            return Json(new { success = true });
        }

        
        
    }
}

