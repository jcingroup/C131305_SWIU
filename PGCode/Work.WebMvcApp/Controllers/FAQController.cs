using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DotWeb.WebApp.Controllers
{
    public class FAQController : WebFrontController
    {
        public ActionResult Index()
        {
            Response.Redirect(Url.Action("FAQ"));
            return View();
        }

        public ActionResult FAQ()
        {
            ViewBag.BodyClass = "FAQ";
            return View();
        }
    }
}
