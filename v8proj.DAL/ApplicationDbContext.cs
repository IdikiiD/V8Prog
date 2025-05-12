using System.Data.Entity;
using v8proj.Core.Entities;
using v8proj.Core.Entities.User;
using v8proj.Core.Entities;
using v8proj.Web.Model;


namespace v8proj.DAL
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<UserEf> Users { get; set; }

        public ApplicationDbContext() : base("DefaultConnection")
        {
        }
        
        public DbSet<eUseControl> eUseControl { get; set; }

        
        public DbSet<Post> Posts { get; set; }

    }
}