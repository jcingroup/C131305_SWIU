using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProcCore.Business.Logic;
namespace DotWeb.WebApp.Controllers
{
    public class IndexController : WebFrontController
    {
        public ActionResult Index()
        {
            FrontWeb fWeb = new FrontWeb();

            a_消息D ac_News = new a_消息D() { Connection = getSQLConnection(), logPlamInfo = plamInfo };
            var r1 = ac_News.SearchMaster(new q_消息D() { MaxRecord=4 }, 0).SearchData;

            a_Schools ac_Schools = new a_Schools() { Connection = getSQLConnection(), logPlamInfo = plamInfo };
            var r2 = ac_Schools.SearchMaster(new q_Schools() { }, 0).SearchData;

            fWeb.news = r1;
            fWeb.schools = r2;

            ViewBag.BodyClass = "index";
            return View(fWeb);
        }

        public ActionResult PageInfo(int kid)
        {
            ViewBag.BodyClass = "page" + kid;
            a_PageData ac_PageData = new a_PageData() { Connection = getSQLConnection(), logPlamInfo = plamInfo };
            m_PageData r = ac_PageData.SearchKid(new q_PageData() { s_kid = kid }, 0).SearchData.FirstOrDefault();

            return View(r);
        }

        public ActionResult SchoolsInfo(int department)
        {
            ViewBag.BodyClass = "page" + department;
            a_Department ac_PageData = new a_Department() { Connection = getSQLConnection(), logPlamInfo = plamInfo };
            m_Department r = ac_PageData.GetDataMaster(department, 0).SearchData;

            ItemsManage itm = new ItemsManage() { Connection = getSQLConnection(), logPlamInfo = plamInfo };
            var r2 = itm.i_Departments(r.kid);

            ViewBag.Option_Department = MakeCollectDataToOptions(r2, true);

            return View(r);
        }
    }
}
