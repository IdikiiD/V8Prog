using System.Linq;
using System.Data.Entity;
using v8proj.Core.Entities;
using v8proj.Core.Interface.Report;
using v8proj.DAL;
using v8proj.DAL.Repositories.Generic;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace v8proj.DAL.Repositories.Report
{
    public class ReportRepository : GenericRepository<Core.Entities.Report>, IReportRepository
    {
        public ReportRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Core.Entities.Report>> GetUnresolvedReportsWithDetailsAsync()
        {
            return await _dbSet.Where(r => !r.IsResolved)
                .Include(r => r.ReportedPost)
                .Include(r => r.ReporterUser)
                .ToListAsync();
        }

        public async Task<IEnumerable<Core.Entities.Report>> GetAllReportsWithDetailsAsync()
        {
            return await _dbSet.Include(r => r.ReportedPost)
                .Include(r => r.ReporterUser)
                .ToListAsync();
        }

        public async Task<Core.Entities.Report> GetReportByIdWithDetailsAsync(int id)
        {
            return await _dbSet.Include(r => r.ReportedPost)
                .Include(r => r.ReporterUser)
                .FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}