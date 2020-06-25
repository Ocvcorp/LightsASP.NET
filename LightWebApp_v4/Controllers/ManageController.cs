using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using LightWebApp_v4.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Data.Entity;

namespace LightWebApp_v4.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

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
        //
        // GET: /Manage/Index
        public async Task<ActionResult>Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Ваш пароль изменен."
                : message == ManageMessageId.SetPasswordSuccess ? "Пароль задан."
                : message == ManageMessageId.ChangeUserInfoSuccess ? "Информация изменена"
                : message == ManageMessageId.Error ? "Произошла ошибка."
                : "";

            var userId = User.Identity.GetUserId();
            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                Email = await UserManager.GetEmailAsync(userId),
                Company = UserManager.FindById(userId).Company,
                ContactPerson = UserManager.FindById(userId).ContactPerson,
                Info = UserManager.FindById(userId).Info
            };
            return View(model);
        }

        [Authorize(Roles ="admin")]
        public ActionResult UsersIndex()
        {
            var model = db.Users.ToList();           
            return View(model);
        }
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> UserDetails(string Name)
        {
            ApplicationUser user = await UserManager.FindByNameAsync(Name);
            if (user != null)
            {
                var model = new IndexViewModel
                {
                    PhoneNumber = await UserManager.GetPhoneNumberAsync(user.Id),
                    Email = await UserManager.GetEmailAsync(user.Id),
                    Company = UserManager.FindById(user.Id).Company,
                    ContactPerson = UserManager.FindById(user.Id).ContactPerson,
                    Info = UserManager.FindById(user.Id).Info
                };
                List<Light> lightNames = db.Lights.Where(l => l.ApplicationUserId == user.Id).ToList();
                ViewBag.Lights = lightNames;
                return View(model);
            }
            return RedirectToAction("UsersIndex", "Manage");
        }
        public async Task<ActionResult> Edit()
        {
            ApplicationUser user = await UserManager.FindByNameAsync(User.Identity.Name);
            if (user != null)
            {
                IndexViewModel model = new IndexViewModel {   Company = user.Company,
                                                            ContactPerson = user.ContactPerson,
                                                            Email = user.Email,
                                                            Info=user.Info};
                return View(model);
            }
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public async Task<ActionResult> Edit(IndexViewModel model)
        {
            ApplicationUser user = await UserManager.FindByNameAsync(User.Identity.Name);
            if (user != null)
            {
                user.Company= model.Company;
                user.ContactPerson = model.ContactPerson;
                user.PhoneNumber = model.PhoneNumber;
                user.Email = model.Email;
                user.Info = model.Info;
                IdentityResult result = await UserManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", new { Message = ManageMessageId.ChangeUserInfoSuccess });
                }
                else
                {
                    ModelState.AddModelError("", "Что-то пошло не так");
                }
            }
            else
            {
                ModelState.AddModelError("", "Пользователь не найден");
            }

            return View(model);
        }
        // GET: /Manage/Delete
        public ActionResult Delete()
        {
            ApplicationUser user = UserManager.FindByName(User.Identity.Name);         
            IndexViewModel model = new IndexViewModel
            {
                Company = user.Company,
                ContactPerson = user.ContactPerson,
                PhoneNumber=user.PhoneNumber,
                Email = user.Email,
                Info = user.Info
            };
            List<Light> lightNames = db.Lights.Where(u => u.ApplicationUserId == user.Id).ToList();
            ViewBag.Lights = lightNames;

            return View(model);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed()
        {
            ApplicationUser user = await UserManager.FindByNameAsync(User.Identity.Name);
            if (user != null)
            {
                List<LightFile> files = db.LightFiles.ToList();
                foreach (Light light in db.Lights)
                {
                    if (light.ApplicationUserId == user.Id)
                    {
                        foreach (LightFile lf in files)
                        {
                            if (lf.Light == light)
                                db.LightFiles.Remove(lf);
                        }
                        db.Lights.Remove(light);
                    }
                }
                db.SaveChanges();
                IdentityResult result = await UserManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Login", "Account");
        }
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // Это сообщение означает наличие ошибки; повторное отображение формы
            return View(model);
        }

#region Вспомогательные приложения
        // Используется для защиты от XSRF-атак при добавлении внешних имен входа
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            ChangeUserInfoSuccess,
            SetPasswordSuccess,
            Error
        }

#endregion
    }
}