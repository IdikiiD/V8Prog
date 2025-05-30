using System.Data.Entity;
using v8proj.Core.Entities;
using v8proj.Core.Entities.User;
using v8proj.Web.Model; 


namespace v8proj.DAL
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<UserEf> Users { get; set; }
        public DbSet<Support> Supports { get; set; }
        public DbSet<eUseControl> eUseControl { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Report> Reports { get; set; }

        public ApplicationDbContext() : base("DefaultConnection")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserEf>()
                .HasMany(u => u.FavoritePosts)
                .WithMany(p => p.FavoritedBy)
                .Map(m =>
                {
                    m.ToTable("UserFavorites");
                    m.MapLeftKey("UserId");
                    m.MapRightKey("PostId");
                });

            modelBuilder.Entity<Report>()
                .HasRequired(r => r.ReportedPost) 
                .WithMany(p => p.Reports)     
                .HasForeignKey(r => r.ReportedPostId) 
                .WillCascadeOnDelete(true);    
          
            modelBuilder.Entity<Report>()
                .HasRequired(r => r.ReporterUser)
                .WithMany() 
                .HasForeignKey(r => r.ReporterUserId)
                .WillCascadeOnDelete(false); 
        }
    }
}