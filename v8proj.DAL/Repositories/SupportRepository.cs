using v8proj.Core.Entities;
using v8proj.Core.Interface.Support;
using v8proj.DAL;

namespace v8proj.DAL.Repositories
{
    public class SupportRepository : ISupportRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public SupportRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(Support support)
        {
            _dbContext.Supports.Add(support);
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
    }
}