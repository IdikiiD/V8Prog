using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using v8proj.Core.Entities.User;

namespace v8proj.Core.Entities
{
    public class Support
    {
        [Key]
        public int SupportId { get; set; }

        [ForeignKey("User")]
        public int? UserId { get; set; }
        public UserEf User { get; set; } // Изменено на UserEf

        [ForeignKey("Post")]
        public int PostId { get; set; }
        public Post Post { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public string Comment { get; set; }

        [Required]
        public string CardNumber { get; set; }

        [Required]
        public string ExpiryMonth { get; set; }

        [Required]
        public string ExpiryYear { get; set; }

        [Required]
        public string CVV { get; set; }

        public DateTime SupportDate { get; set; } = DateTime.UtcNow;
    }
}