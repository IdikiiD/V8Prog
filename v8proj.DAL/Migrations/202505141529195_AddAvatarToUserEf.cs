namespace v8proj.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAvatarToUserEf : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserEfs", "AvatarUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserEfs", "AvatarUrl");
        }
    }
}
