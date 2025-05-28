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
                "dbo.Reports",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ReportedPostId = c.Int(nullable: false),
                        ReporterUserId = c.Int(nullable: false),
                        Reason = c.String(nullable: false, maxLength: 500),
                        ReportDate = c.DateTime(nullable: false),
                        IsResolved = c.Boolean(nullable: false),
                        ResolutionDate = c.DateTime(),
                        ResolutionDetails = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Posts", t => t.ReportedPostId, cascadeDelete: true)
                .ForeignKey("dbo.UserEfs", t => t.ReporterUserId, cascadeDelete: true)
                .Index(t => t.ReportedPostId)
                .Index(t => t.ReporterUserId);
            
            CreateTable(
                "dbo.Supports",
                c => new
                    {
                        SupportId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(),
                        PostId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Comment = c.String(),
                        CardNumber = c.String(nullable: false),
                        ExpiryMonth = c.String(nullable: false),
                        ExpiryYear = c.String(nullable: false),
                        CVV = c.String(nullable: false),
                        SupportDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.SupportId)
                .ForeignKey("dbo.Posts", t => t.PostId, cascadeDelete: true)
                .ForeignKey("dbo.UserEfs", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.PostId);
            
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
            DropForeignKey("dbo.Supports", "UserId", "dbo.UserEfs");
            DropForeignKey("dbo.Supports", "PostId", "dbo.Posts");
            DropForeignKey("dbo.Reports", "ReporterUserId", "dbo.UserEfs");
            DropForeignKey("dbo.Reports", "ReportedPostId", "dbo.Posts");
            DropForeignKey("dbo.UserFavorites", "PostId", "dbo.Posts");
            DropForeignKey("dbo.UserFavorites", "UserId", "dbo.UserEfs");
            DropIndex("dbo.UserFavorites", new[] { "PostId" });
            DropIndex("dbo.UserFavorites", new[] { "UserId" });
            DropIndex("dbo.Supports", new[] { "PostId" });
            DropIndex("dbo.Supports", new[] { "UserId" });
            DropIndex("dbo.Reports", new[] { "ReporterUserId" });
            DropIndex("dbo.Reports", new[] { "ReportedPostId" });
            DropTable("dbo.UserFavorites");
            DropTable("dbo.Supports");
            DropTable("dbo.Reports");
            DropTable("dbo.UserEfs");
            DropTable("dbo.Posts");
            DropTable("dbo.eUseControls");
        }
    }
}
