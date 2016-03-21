using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Configuration;
using System.Data.SqlClient;
using System.Web.Script.Serialization;

using System.Data;
using ProcCore.DatabaseCore.DataBaseConnection;
using ProcCore.Business.Base;
using ProcCore.Business.Logic;

namespace DotWeb
{
    /// <summary>
    /// WebSrv 的摘要描述
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下一行。
    [System.Web.Script.Services.ScriptService]
    public class WebSrv : System.Web.Services.WebService
    {
        private CommConnection Connection;
        private void OpenConnection()
        {
            BaseConnection BConn = new BaseConnection();
            BConn.Server = System.Configuration.ConfigurationManager.AppSettings["Server"];
            BConn.Account = System.Configuration.ConfigurationManager.AppSettings["Account"];
            BConn.Password = System.Configuration.ConfigurationManager.AppSettings["Password"];

            this.Connection = BConn.GetConnection();
        }
        private void CloseConnection()
        {
            this.Connection.Close();
        }
    }
}
