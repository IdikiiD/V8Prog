using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using v8proj.Core.Entities.User;

namespace v8proj.Core.Entities
{
    public class Report
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ReportedPostId { get; set; }
        [ForeignKey("ReportedPostId")]
        public Post ReportedPost { get; set; }

        public int ReporterUserId { get; set; }
        [ForeignKey("ReporterUserId")]
        public UserEf ReporterUser { get; set; }

        [Required]
        [StringLength(500)]
        public string Reason { get; set; }

        public DateTime ReportDate { get; set; } = DateTime.UtcNow;

        public bool IsResolved { get; set; } = false;
        public DateTime? ResolutionDate { get; set; }
        public string ResolutionDetails { get; set; }
    }
}