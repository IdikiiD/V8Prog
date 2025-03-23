using System.Data.Entity;
using v8proj.Core.Entities.User;


namespace v8proj.DAL
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<UserEf> Users { get; set; }

        public ApplicationDbContext() : base("DefaultConnection")
        {
        }
    }
}