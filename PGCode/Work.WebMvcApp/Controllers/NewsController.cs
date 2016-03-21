using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProcCore.Business.Logic;

namespace DotWeb.WebApp.Controllers
{
    public class NewsController : WebFrontController
    {
        public NewsController()
        {
            ViewBag.BodyClass = "News";
        }

        public ActionResult Index()
        {

            Response.Redirect(Url.Action("News"));
            return View();
        }

        public ActionResult News()
        {
            a_消息D ac_消息D = new a_消息D() { Connection=getSQLConnection(),logPlamInfo=this.plamInfo };
            var r1 = ac_消息D.SearchMaster(new q_消息D() { s_isopen=true },0);
            return View(r1.SearchData);
        }

        public ActionResult News_content(int id)
        {
            a_消息D ac_消息D = new a_消息D() { Connection = getSQLConnection(), logPlamInfo = this.plamInfo };
            var r1 = ac_消息D.GetDataMaster(id, 0);
            return View(r1.SearchData);
        }

    }
}
