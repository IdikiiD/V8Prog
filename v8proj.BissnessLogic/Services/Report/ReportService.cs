using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity; 
using v8proj.BissnessLogic.Interfaces.Reports;
using v8proj.Core.Entities;
using v8proj.BissnessLogic.Interfaces.Posts;
using v8proj.BissnessLogic.Interfaces.User;
using v8proj.Web.Model.DTO;
using v8proj.Core.Model.DTO.Report;
using v8proj.Core.Enums;
using v8proj.DAL; 

namespace v8proj.BissnessLogic.Services.Reports
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context; 
        private readonly IPostService _postService;
        private readonly IUserService _userService;

        public ReportService(ApplicationDbContext context, IPostService postService, IUserService userService) // ИЗМЕНЕН КОНСТРУКТОР
        {
            _context = context; 
            _postService = postService;
            _userService = userService;
        }

        public async Task<BaseResponse<object>> CreateReport(CreateReportDto reportDto, int reporterUserId)
        {
            var post = _postService.GetPostById(reportDto.PostId);
            if (post == null)
            {
                return new BaseResponse<object>(null, OperationStatus.Error, "Пост не найден.");
            }

            var reporterUserResponse = await _userService.GetUserByIdAsync(reporterUserId);
            if (reporterUserResponse.Status != OperationStatus.Success || reporterUserResponse.Data == null)
            {
                return new BaseResponse<object>(null, OperationStatus.Error, "Пользователь, подающий жалобу, не найден.");
            }

            var report = new Report
            {
                ReportedPostId = reportDto.PostId,
                ReporterUserId = reporterUserId,
                Reason = reportDto.Reason,
                ReportDate = DateTime.UtcNow,
                IsResolved = false
            };

            _context.Reports.Add(report); // Используем DbSet
            await _context.SaveChangesAsync(); // Сохраняем изменения

            return new BaseResponse<object>(null, OperationStatus.Success, "Жалоба успешно отправлена.");
        }

        public async Task<IEnumerable<ReportViewDto>> GetUnresolvedReports()
        {
            // Здесь мы выполняем те же Include, что были в ReportRepository
            var reports = await _context.Reports
                                 .Where(r => !r.IsResolved)
                                 .Include(r => r.ReportedPost)
                                 .Include(r => r.ReporterUser)
                                 .ToListAsync();

            return reports?.Select(r => new ReportViewDto
            {
                Id = r.Id,
                ReportedPostId = r.ReportedPostId,
                ReportedPostTitle = r.ReportedPost?.Title,
                ReporterUserId = r.ReporterUserId,
                ReporterUserName = r.ReporterUser?.FullName,
                Reason = r.Reason,
                ReportDate = r.ReportDate,
                IsResolved = r.IsResolved
            }).OrderByDescending(r => r.ReportDate) ?? Enumerable.Empty<ReportViewDto>();
        }

        public async Task<IEnumerable<ReportViewDto>> GetAllReports()
        {
            // Здесь мы выполняем те же Include, что были в ReportRepository
            var reports = await _context.Reports
                                 .Include(r => r.ReportedPost)
                                 .Include(r => r.ReporterUser)
                                 .ToListAsync();

            return reports?.Select(r => new ReportViewDto
            {
                Id = r.Id,
                ReportedPostId = r.ReportedPostId,
                ReportedPostTitle = r.ReportedPost?.Title,
                ReporterUserId = r.ReporterUserId,
                ReporterUserName = r.ReporterUser?.FullName,
                Reason = r.Reason,
                ReportDate = r.ReportDate,
                IsResolved = r.IsResolved,
                ResolutionDate = r.ResolutionDate,
                ResolutionDetails = r.ResolutionDetails
            }).OrderByDescending(r => r.ReportDate) ?? Enumerable.Empty<ReportViewDto>();
        }

        public async Task<BaseResponse<object>> ResolveReport(int reportId, string resolutionDetails)
        {
            var report = await _context.Reports.FindAsync(reportId); // Используем FindAsync из DbSet

            if (report == null)
            {
                return new BaseResponse<object>(null, OperationStatus.Error, "Жалоба не найдена.");
            }

            if (report.IsResolved)
            {
                return new BaseResponse<object>(null, OperationStatus.Error, "Эта жалоба уже была рассмотрена.");
            }

            report.IsResolved = true;
            report.ResolutionDate = DateTime.UtcNow;
            report.ResolutionDetails = resolutionDetails;

            // _context.Entry(report).State = EntityState.Modified; // FindAsync уже отслеживает сущность
            await _context.SaveChangesAsync(); // Сохраняем изменения

            return new BaseResponse<object>(null, OperationStatus.Success, "Жалоба успешно рассмотрена.");
        }

        public async Task<ReportViewDto> GetReportById(int reportId)
        {
             // Здесь мы выполняем те же Include, что были в ReportRepository
             var report = await _context.Reports
                                  .Include(r => r.ReportedPost)
                                  .Include(r => r.ReporterUser)
                                  .FirstOrDefaultAsync(r => r.Id == reportId);

             if (report == null)
                 return null;

             return new ReportViewDto
             {
                Id = report.Id,
                ReportedPostId = report.ReportedPostId,
                ReportedPostTitle = report.ReportedPost?.Title,
                ReporterUserId = report.ReporterUserId,
                ReporterUserName = report.ReporterUser?.FullName,
                Reason = report.Reason,
                ReportDate = report.ReportDate,
                IsResolved = report.IsResolved,
                ResolutionDate = report.ResolutionDate,
                ResolutionDetails = report.ResolutionDetails
             };
        }
    }
}