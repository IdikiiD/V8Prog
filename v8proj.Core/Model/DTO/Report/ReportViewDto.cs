using System;

namespace v8proj.Core.Model.DTO.Report
{
    public class ReportViewDto
    {
        public int Id { get; set; }
        public int ReportedPostId { get; set; }
        public string ReportedPostTitle { get; set; }
        public int ReporterUserId { get; set; }
        public string ReporterUserName { get; set; }
        public string Reason { get; set; }
        public DateTime ReportDate { get; set; }
        public bool IsResolved { get; set; }
        public DateTime? ResolutionDate { get; set; }
        public string ResolutionDetails { get; set; }
    }
}