namespace LightWebApp_v4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DataMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Businesses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Lights",
                c => new
                    {
                        LightId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Adress = c.String(),
                        Description = c.String(),
                        ApplicationUserId = c.String(maxLength: 128),
                        StageId = c.Int(),
                        LightTypeId = c.Int(),
                        ProjectSetId = c.Int(),
                        UseFieldId = c.Int(),
                        BusinessId = c.Int(),
                    })
                .PrimaryKey(t => t.LightId)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .ForeignKey("dbo.Businesses", t => t.BusinessId)
                .ForeignKey("dbo.LightTypes", t => t.LightTypeId)
                .ForeignKey("dbo.UseFields", t => t.UseFieldId)
                .ForeignKey("dbo.ProjectSets", t => t.ProjectSetId)
                .ForeignKey("dbo.Stages", t => t.StageId)
                .Index(t => t.ApplicationUserId)
                .Index(t => t.StageId)
                .Index(t => t.LightTypeId)
                .Index(t => t.ProjectSetId)
                .Index(t => t.UseFieldId)
                .Index(t => t.BusinessId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Company = c.String(),
                        ContactPerson = c.String(),
                        Info = c.String(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.LightFiles",
                c => new
                    {
                        LightFileId = c.Int(nullable: false, identity: true),
                        FileName = c.String(),
                        MimeType = c.String(),
                        File = c.Binary(),
                        LightId = c.Int(),
                    })
                .PrimaryKey(t => t.LightFileId)
                .ForeignKey("dbo.Lights", t => t.LightId)
                .Index(t => t.LightId);
            
            CreateTable(
                "dbo.LightTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UseFields",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        LightTypeId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.LightTypes", t => t.LightTypeId)
                .Index(t => t.LightTypeId);
            
            CreateTable(
                "dbo.ProjectSets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Stages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Lights", "StageId", "dbo.Stages");
            DropForeignKey("dbo.Lights", "ProjectSetId", "dbo.ProjectSets");
            DropForeignKey("dbo.UseFields", "LightTypeId", "dbo.LightTypes");
            DropForeignKey("dbo.Lights", "UseFieldId", "dbo.UseFields");
            DropForeignKey("dbo.Lights", "LightTypeId", "dbo.LightTypes");
            DropForeignKey("dbo.LightFiles", "LightId", "dbo.Lights");
            DropForeignKey("dbo.Lights", "BusinessId", "dbo.Businesses");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Lights", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.UseFields", new[] { "LightTypeId" });
            DropIndex("dbo.LightFiles", new[] { "LightId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Lights", new[] { "BusinessId" });
            DropIndex("dbo.Lights", new[] { "UseFieldId" });
            DropIndex("dbo.Lights", new[] { "ProjectSetId" });
            DropIndex("dbo.Lights", new[] { "LightTypeId" });
            DropIndex("dbo.Lights", new[] { "StageId" });
            DropIndex("dbo.Lights", new[] { "ApplicationUserId" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Stages");
            DropTable("dbo.ProjectSets");
            DropTable("dbo.UseFields");
            DropTable("dbo.LightTypes");
            DropTable("dbo.LightFiles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Lights");
            DropTable("dbo.Businesses");
        }
    }
}
