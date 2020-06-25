using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LightWebApp_v4.Models
{
    public class ApplicationUser : IdentityUser
    {
        // добавляем свойства
        public string Company { get; set; }
        public string ContactPerson { get; set; }
        public string Info { get; set; }
        public ICollection<Light> Lights { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // authenticationType должен совпадать с типом, определенным в CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }
    }
    //добавляем классы
    public class Light
    {
        public int LightId { get; set; }
        public string Name { get; set; }
        public string Adress { get; set; }
        public string Description { get; set; }

        //ссылки
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public int? StageId { get; set; }
        public Stage Stage { get; set; }
        public int? LightTypeId { get; set; }
        public LightType LightType { get; set; }
        public int? ProjectSetId { get; set; }
        public ProjectSet ProjectSet { get; set; }
        public int? UseFieldId { get; set; }
        public UseField UseField { get; set; }
        public int? BusinessId { get; set; }
        public Business Business { get; set; }
        public ICollection<LightFile> LightFiles { get; set; }
    }
    public class LightFile
    {
        public int LightFileId { get; set; }
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public byte[] File { get; set; }
        //ссылка на проект
        public int? LightId { get; set; }
        public Light Light { get; set; }
    }
    public class Stage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Light> Lights { get; set; }
    }
    public class ProjectSet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Light> Lights { get; set; }
    }
    public class Business
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Light> Lights { get; set; }
    }
    public class LightType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<UseField> UseFields { get; set; }
        public ICollection<Light> Lights { get; set; }
    }
    public class UseField
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? LightTypeId { get; set; }
        public LightType LightType { get; set; }
        public ICollection<Light> Lights { get; set; }
    }
    
    
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Light> Lights { get; set; }
        public DbSet<Stage> Stages { get; set; }
        public DbSet<ProjectSet> ProjectSets { get; set; }
        public DbSet<LightType> LightTypes { get; set; }
        public DbSet<UseField> UseFields { get; set; }
        public DbSet<Business> Businesses { get; set; }
        public DbSet<LightFile> LightFiles { get; set; }
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}