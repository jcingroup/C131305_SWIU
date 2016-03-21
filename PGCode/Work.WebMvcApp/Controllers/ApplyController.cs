using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProcCore.Business.Logic;
using Newtonsoft.Json;
using System.Net.Mail;


namespace DotWeb.WebApp.Controllers
{
    public class ApplyController : WebFrontController
    {
        public ActionResult Index()
        {
            Response.Redirect(Url.Action("Apply"));
            return View();
        }

        public ActionResult Apply()
        {
            Session.Remove("maildata");
            ViewBag.BodyClass = "page11";
            return View();
        }

        public ActionResult Apply2(Apply ap)
        {
            a_Schools ac_Schools = new a_Schools() { Connection = getSQLConnection(), logPlamInfo = this.plamInfo };
            ap.ex_school = ac_Schools.GetDataMaster(ap.school, 0).SearchData.name;

            a_Department ac_Department = new a_Department() { Connection = this.getSQLConnection(), logPlamInfo = this.plamInfo };
            ap.ex_department = ac_Department.GetDataMaster(ap.department, 0).SearchData.name;

            ViewBag.BodyClass = "page11";
            Session["maildata"] = ap;
            return View(ap);
        }

        public string ajax_sendmail() {

            try
            {
                Apply ap = (Apply)Session["maildata"];

                ViewResult resultView = View("mail_apply", ap);

                StringResult sr = new StringResult();
                sr.ViewName = resultView.ViewName;
                sr.MasterName = resultView.MasterName;
                sr.ViewData = resultView.ViewData;
                sr.TempData = resultView.TempData;
                sr.ExecuteResult(this.ControllerContext);

                MailMessage message = new MailMessage();

                message.From = new MailAddress(ap.email,ap.name);
                message.To.Add(new MailAddress("henryyeh@hotmail.com", "henryyeh"));
                message.To.Add(new MailAddress("king@jcin.com.tw", "KingA"));
                message.To.Add(new MailAddress("king_lu@xuite.net", "KingB"));
                message.To.Add(new MailAddress("lei.chieh@gmail.com", "Stacey"));

                message.CC.Add(new MailAddress("jerryissky@gmail.com", "Jerry"));

                message.IsBodyHtml = true;
                message.BodyEncoding = System.Text.Encoding.UTF8;//E-mail編碼
                message.Subject = Resources.ResWeb.MailSubjectApply;
                message.Body = sr.ToHtmlString;

                String MailServer = System.Configuration.ConfigurationManager.AppSettings["MailServer"];

                SmtpClient smtpClient = new SmtpClient(MailServer, 25); //設定E-mail Server和port
                smtpClient.Send(message);
                return JsonConvert.SerializeObject(true, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(false, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            }
            finally {
                Session.Remove("maildata");
            }
        }
    }
}
