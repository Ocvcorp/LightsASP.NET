using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LightWebApp_v4.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult About()
        {
            ViewBag.Message = "Описание работы";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Разработка C#";

            return View();
        }
    }
}