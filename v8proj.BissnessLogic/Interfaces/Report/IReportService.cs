using System.Collections.Generic;
using System.Threading.Tasks;
using v8proj.Core.Model.DTO.Report;
using v8proj.Web.Model.DTO; 

namespace v8proj.BissnessLogic.Interfaces.Reports
{
    public interface IReportService
    {
        Task<BaseResponse<object>> CreateReport(CreateReportDto reportDto, int reporterUserId);
        Task<IEnumerable<ReportViewDto>> GetUnresolvedReports();
        Task<IEnumerable<ReportViewDto>> GetAllReports();
        Task<BaseResponse<object>> ResolveReport(int reportId, string resolutionDetails);
        Task<ReportViewDto> GetReportById(int reportId);
    }
}