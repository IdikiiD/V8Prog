namespace v8proj.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSupportTable : DbMigration
    {
        public override void Up()
        {
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Supports", "UserId", "dbo.UserEfs");
            DropForeignKey("dbo.Supports", "PostId", "dbo.Posts");
            DropIndex("dbo.Supports", new[] { "PostId" });
            DropIndex("dbo.Supports", new[] { "UserId" });
            DropTable("dbo.Supports");
        }
    }
}
