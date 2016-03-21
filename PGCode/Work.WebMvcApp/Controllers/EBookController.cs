using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DotWeb.WebApp.Controllers
{
    public class EBookController : WebFrontController
    {
        public ActionResult Index()
        {
            Response.Redirect(Url.Action("EBook"));
            return View();
        }

        public ActionResult EBook()
        {
            ViewBag.BodyClass = "EBook";
            return View();
        }
    }
}
