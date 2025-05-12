using System.ComponentModel.DataAnnotations;
using System.Web;

namespace v8proj.Web.Model.ViewModels
{
    public class PostCreateViewModel
    {
        [Required(ErrorMessage = "Поле 'Название' обязательно для заполнения.")]
        public string Title { get; set; }

        public string Description { get; set; }
        public string Category { get; set; }

        public HttpPostedFileBase Image1 { get; set; }
        public HttpPostedFileBase Image2 { get; set; }
        public HttpPostedFileBase Image3 { get; set; }
    }
}