using System.ComponentModel.DataAnnotations;

namespace v8proj.Core.Model.DTO.Report
{
    public class CreateReportDto
    {
        [Required(ErrorMessage = "ID поста обязателен.")]
        public int PostId { get; set; }

        [Required(ErrorMessage = "Причина жалобы обязательна.")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Причина должна быть от 10 до 500 символов.")]
        public string Reason { get; set; }
    }
}