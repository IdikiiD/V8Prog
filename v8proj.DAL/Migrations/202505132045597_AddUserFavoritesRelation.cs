namespace v8proj.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserFavoritesRelation : DbMigration
    {
        public override void Up()
        {
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
        }
    }
}
