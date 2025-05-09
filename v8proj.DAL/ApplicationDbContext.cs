using System.Data.Entity;
using v8proj.Core.Entities.User;
using v8proj.Core.Model.ViewModels; // Добавьте пространство имен для eUseControl

namespace v8proj.DAL
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<UserEf> Users { get; set; }
        public DbSet<eUseControl> eUseControls { get; set; } // Добавьте DbSet для eUseControl

        public ApplicationDbContext() : base("DefaultConnection")
        {
        }
    }
}