using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using LightWebApp_v4.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Net;
using System.IO;

namespace LightWebApp_v4.Controllers
{
    public class LightsController : Controller
    {
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Lights
        [Authorize]
        public ActionResult Index()
        {
            List<Light> lights;
            string userId = UserManager.FindById(User.Identity.GetUserId()).Id; 
            string role = UserManager.GetRoles(userId)[0];
            if (role == "admin")
            {
                lights = db.Lights
                      .Include(ua => ua.ApplicationUser)
                      .Include(s => s.Stage)
                      .Include(ps => ps.ProjectSet)
                      .ToList();               
            }
            else
            {
                lights = db.Lights.Where(u => u.ApplicationUserId == userId)
                   .Include(ua => ua.ApplicationUser)
                   .Include(s => s.Stage)
                   .Include(ps => ps.ProjectSet)
                   .ToList();
            }
            return View(lights);
        }
        [Authorize]
        public ActionResult Files(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Light light = db.Lights.Include(l => l.LightFiles).FirstOrDefault(l => l.LightId == id);
            if (light == null)
            {
                return HttpNotFound();
            }
            return View(light);
        }
        [Authorize]
        public ActionResult AddFile(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Light light = db.Lights.Find(id);
            if (light == null)
            {
                return HttpNotFound();
            }
            return View(light);
        }
        [HttpPost]
        [Authorize]
        public ActionResult AddFile(LightFile file, HttpPostedFileBase postedFile, int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (ModelState.IsValid && postedFile != null)
            {
                byte[] fileData = null;
                // считываем переданный файл в массив байтов
                using (var binaryReader = new BinaryReader(postedFile.InputStream))
                {
                    fileData = binaryReader.ReadBytes(postedFile.ContentLength);
                }
                // установка массива байтов
                file.File = fileData;
                // остальные поля
                file.MimeType = postedFile.ContentType;
                file.FileName = Path.GetFileName(postedFile.FileName);
                file.Light = db.Lights.Find(id);
                db.LightFiles.Add(file);
                db.SaveChanges();
                return RedirectToAction("Files", "Lights", new { id });
            }
            return View();
        }
        [Authorize]
        public ActionResult GetFile(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LightFile lightFile = db.LightFiles.Find(id);
            FileContentResult file = new FileContentResult(lightFile.File, lightFile.MimeType);
            file.FileDownloadName = lightFile.FileName;
            if (lightFile == null)
            {
                return HttpNotFound();
            }
            return file;
        }


        // GET: Lights/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Light light = db.Lights.Find(id);
            ViewBag.Stage = db.Stages.Find(light.StageId).Name;
            ViewBag.ProjectSet = db.ProjectSets.Find(light.ProjectSetId).Name;
            ViewBag.Company = UserManager.FindById(User.Identity.GetUserId()).Company;
            ViewBag.LightType = db.LightTypes.Find(light.LightTypeId).Name;
            ViewBag.Business = db.Businesses.Find(light.BusinessId).Name;
            if (light == null)
            {
                return HttpNotFound();
            }
            return View(light);
        }

        // GET: Lights/Create
        [Authorize(Roles = "user")]
        public ActionResult Create()
        {
            SelectList stages = new SelectList(db.Stages, "Id", "Name");
            ViewBag.Stages = stages;
            SelectList businesses = new SelectList(db.Businesses, "Id", "Name");
            ViewBag.Businesses = businesses;
            SelectList projectSets = new SelectList(db.ProjectSets, "Id", "Name");
            ViewBag.ProjectSets = projectSets;

            int indLightType = 2;
            SelectList lightTypes = new SelectList(db.LightTypes, "Id", "Name", indLightType);
            ViewBag.LightTypes = lightTypes;
            SelectList useFields = new SelectList(db.UseFields.Where(u => u.LightTypeId == indLightType), "Id", "Name");
            ViewBag.UseFields = useFields;
            return View();
        }
        [Authorize]
        public ActionResult GetItems(int id)
        {
            return PartialView(db.UseFields.Where(u => u.LightTypeId == id).ToList());
        }
        // POST: Lights/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "user")]
        public ActionResult Create([Bind(Include = "LightId,Name,Adress,Description,StageId,ProjectSetId,LightTypeId,UseFieldId,BusinessId")] Light light)
        {
            if (ModelState.IsValid)
            {
                light.ApplicationUserId = UserManager.FindById(User.Identity.GetUserId()).Id;
                db.Lights.Add(light);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(light);
        }

        // GET: Lights/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Light light = db.Lights.Find(id);
            SelectList stages = new SelectList(db.Stages, "Id", "Name", light.StageId);
            ViewBag.Stages = stages;
            SelectList businesses = new SelectList(db.Businesses, "Id", "Name", light.BusinessId);
            ViewBag.Businesses = businesses;
            SelectList projectSets = new SelectList(db.ProjectSets, "Id", "Name", light.ProjectSetId);
            ViewBag.ProjectSets = projectSets;
            SelectList lightTypes = new SelectList(db.LightTypes, "Id", "Name", light.LightTypeId);
            ViewBag.LightTypes = lightTypes;
            SelectList useFields = new SelectList(db.UseFields.Where(u => u.LightTypeId == light.LightTypeId), "Id", "Name");           
            ViewBag.UseFields = useFields;
            if (light == null)
            {
                return HttpNotFound();
            }
            return View(light);
        }

        // POST: Lights/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "LightId,Name,Adress,Description,StageId,ProjectSetId,LightTypeId,UseFieldId,BusinessId")] Light light)
        {
            if (ModelState.IsValid)
            {
                light.ApplicationUserId = UserManager.FindById(User.Identity.GetUserId()).Id;
                db.Entry(light).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(light);
        }

        // GET: Lights/Delete/5
        [Authorize(Roles ="user")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Light light = db.Lights.Find(id);
            ViewBag.Stage = db.Stages.Find(light.StageId).Name;
            ViewBag.ProjectSet = db.ProjectSets.Find(light.ProjectSetId).Name;
            ViewBag.Company = UserManager.FindById(User.Identity.GetUserId()).Company;
            if (light == null)
            {
                return HttpNotFound();
            }
            return View(light);
        }

        // POST: Lights/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "user")]
        public ActionResult DeleteConfirmed(int id)
        {
            Light light = db.Lights.Find(id);
            foreach (LightFile lf in db.LightFiles)
            {
                if (lf.Light == light)
                    db.LightFiles.Remove(lf);
            }
            db.Lights.Remove(light);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}