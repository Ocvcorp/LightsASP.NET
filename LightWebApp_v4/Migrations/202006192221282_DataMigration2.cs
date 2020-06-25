namespace LightWebApp_v4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DataMigration2 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Lights", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.Lights", "ApplicationUserId");
            RenameColumn(table: "dbo.Lights", name: "ApplicationUser_Id", newName: "ApplicationUserId");
            AlterColumn("dbo.Lights", "ApplicationUserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Lights", "ApplicationUserId");
            DropColumn("dbo.AspNetUsers", "Age");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "Age", c => c.Int(nullable: false));
            DropIndex("dbo.Lights", new[] { "ApplicationUserId" });
            AlterColumn("dbo.Lights", "ApplicationUserId", c => c.Int());
            RenameColumn(table: "dbo.Lights", name: "ApplicationUserId", newName: "ApplicationUser_Id");
            AddColumn("dbo.Lights", "ApplicationUserId", c => c.Int());
            CreateIndex("dbo.Lights", "ApplicationUser_Id");
        }
    }
}
