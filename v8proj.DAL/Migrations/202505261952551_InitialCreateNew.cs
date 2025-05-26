namespace v8proj.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreateNew : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.eUseControls",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CartName = c.String(),
                        CartDescription = c.String(),
                        CartPrice = c.Int(nullable: false),
                        CartImage = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Posts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        Category = c.String(),
                        ImagePath1 = c.String(),
                        ImagePath2 = c.String(),
                        ImagePath3 = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserEfs",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        FullName = c.String(),
                        Email = c.String(),
                        PasswordHash = c.String(),
                        AvatarUrl = c.String(),
                        PhoneNumber = c.String(),
                        RegistrationDate = c.DateTime(nullable: false),
                        DateRegistered = c.DateTime(nullable: false),
                        UserType = c.Int(nullable: false),
                        UserStatus = c.Int(nullable: false),
                        IsVerified = c.Int(nullable: false),
                        IsSignUpForLetters = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.UserFavorites",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        PostId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.PostId })
                .ForeignKey("dbo.UserEfs", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Posts", t => t.PostId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.PostId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserFavorites", "PostId", "dbo.Posts");
            DropForeignKey("dbo.UserFavorites", "UserId", "dbo.UserEfs");
            DropIndex("dbo.UserFavorites", new[] { "PostId" });
            DropIndex("dbo.UserFavorites", new[] { "UserId" });
            DropTable("dbo.UserFavorites");
            DropTable("dbo.UserEfs");
            DropTable("dbo.Posts");
            DropTable("dbo.eUseControls");
        }
    }
}
