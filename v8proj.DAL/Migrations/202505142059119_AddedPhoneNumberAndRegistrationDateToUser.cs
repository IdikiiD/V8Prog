namespace v8proj.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPhoneNumberAndRegistrationDateToUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserEfs", "PhoneNumber", c => c.String());
            AddColumn("dbo.UserEfs", "RegistrationDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserEfs", "RegistrationDate");
            DropColumn("dbo.UserEfs", "PhoneNumber");
        }
    }
}
