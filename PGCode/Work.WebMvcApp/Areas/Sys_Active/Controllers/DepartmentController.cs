using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ProcCore;
using ProcCore.WebCore;
using ProcCore.NetExtension;
using ProcCore.Business.Logic;
using ProcCore.Business.Logic.TablesDescription;
using ProcCore.ReturnAjaxResult;
using ProcCore.JqueryHelp.JQGridScript;
using DotWeb.CommSetup;
using Newtonsoft.Json;
using ProcCore.JqueryHelp.ztreeHelp;

namespace DotWeb.Areas.Sys_Active.Controllers
{
    public class SchoolsController : BaseActionSub<m_Schools, a_Schools, q_Schools, Schools, m_Department, a_Department, q_Department, Department>
    {
        #region Action and function section
        public RedirectResult Index()
        {
            return Redirect(Url.Action("ListGrid"));
        }
        public override ActionResult ListGrid(q_Schools sh)
        {
            operationMode = OperationMode.EnterList;
            HandleRequest HRq = new HandleRequest(); //記錄QueryString            
            HRq.encodeURIComponent = true;
            HRq.Remove("page");

            ViewBag.Page = QueryGridPage();
            ViewBag.Caption = GetSystemInfo().prog_name;
            ViewBag.AppendQuertString = HRq.ToQueryString();
            HRq = null;

            return View("ListData", sh);
        }
        public override ActionResult EditMasterNewData()
        {
            operationMode = OperationMode.EditInsert;

            ac = new a_Schools() { Connection = getSQLConnection(), logPlamInfo = plamInfo };
            md = new m_Schools() { id = ac.GetIDX() };
            md.EditType = EditModeType.Insert;
            #region 新增欄位預設值設定
            //md.SetDate = DateTime.Now;
            #endregion
            HandleCollectDataToOptions();

            ViewBag.Caption = GetSystemInfo().prog_name;

            HandleRequest HRq = new HandleRequest();  //記錄QueryString
            HRq.Remove("Id"); //不需記ID
            ViewBag.QueryString = HRq.ToQueryString();
            HRq = null;

            return View("EditData", md);
        }
        public override ActionResult EditMasterDataByID(int id)
        {
            operationMode = OperationMode.EditModify;
            ac = new a_Schools() { Connection = getSQLConnection(), logPlamInfo = plamInfo }; ;

            RunOneDataEnd<m_Schools> HResult = ac.GetDataMaster(id, LoginUserId);
            md = HResult.SearchData;
            md.EditType = EditModeType.Modify;
            HandleResultCheck(HResult);
            HandleCollectDataToOptions();

            ViewBag.Caption = GetSystemInfo().prog_name;

            HandleRequest HRq = new HandleRequest();  //記錄QueryString
            HRq.Remove("id"); //不需記ID
            ViewBag.QueryString = HRq.ToQueryString();
            HRq = null;

            return View("EditData", md);
        }

        public ActionResult EditSubNewData(int sid)
        {
            operationMode = OperationMode.EditInsert;

            acd = new a_Department() { Connection = getSQLConnection(), logPlamInfo = plamInfo };
            mdd = new m_Department() { ids = acd.GetIDX() };
            mdd.kid = sid;
            mdd.EditType = EditModeType.Insert;
            #region 新增欄位預設值設定
            //md.SetDate = DateTime.Now;
            #endregion
            HandleCollectDataToOptions();

            ViewBag.Caption = GetSystemInfo().prog_name;

            HandleRequest HRq = new HandleRequest();  //記錄QueryString
            HRq.Remove("Id"); //不需記ID
            ViewBag.QueryString = HRq.ToQueryString();
            HRq = null;

            return View("EditSub", mdd);
        }
        public ActionResult EditSubDataByID(int id)
        {
            operationMode = OperationMode.EditModify;
            acd = new a_Department() { Connection = getSQLConnection(), logPlamInfo = plamInfo }; ;

            RunOneDataEnd<m_Department> HResult = acd.GetDataMaster(id, LoginUserId);
            mdd = HResult.SearchData;
            mdd.EditType = EditModeType.Modify;
            HandleResultCheck(HResult);
            HandleCollectDataToOptions();

            ViewBag.Caption = GetSystemInfo().prog_name;

            HandleRequest HRq = new HandleRequest();  //記錄QueryString
            HRq.Remove("id"); //不需記ID
            ViewBag.QueryString = HRq.ToQueryString();
            HRq = null;

            return View("EditSub", mdd);
        }
        protected override void HandleCollectDataToOptions()
        {
           // ViewBag.NewsKind_Option = MakeCollectDataToOptions(CodeSheet.NewsKind.ToDictionary(), false);
        }
        #endregion

        #region ajax call section
        [HttpPost]
        [ValidateInput(false)]
        public String ajax_MasterUpdata(m_Schools md)
        {
            ReturnAjaxFiles rAjaxResult = new ReturnAjaxFiles();
            String returnPicturePath = String.Empty;

            ac = new a_Schools() { Connection = getSQLConnection(), logPlamInfo = plamInfo }; ;

            if (md.EditType == EditModeType.Insert)
            {   //新增
                RunInsertEnd HResult = this.ac.InsertMaster(md, LoginUserId);
                rAjaxResult = HandleResultAjaxFiles(HResult, Resources.Res.Data_Insert_Success);
                rAjaxResult.id = HResult.InsertId;
            }
            else
            {
                //修改
                RunEnd HResult = this.ac.UpdateMaster(md, LoginUserId);
                rAjaxResult = HandleResultAjaxFiles(HResult, Resources.Res.Data_Update_Success);
                rAjaxResult.id = md.id;
            }
            return JsonConvert.SerializeObject(rAjaxResult, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
        }

        [HttpPost]
        [ValidateInput(false)]
        public String ajax_SubUpdata(m_Department mdd)
        {
            ReturnAjaxFiles rAjaxResult = new ReturnAjaxFiles();
            String returnPicturePath = String.Empty;

            acd = new a_Department() { Connection = getSQLConnection(), logPlamInfo = plamInfo }; ;

            if (mdd.EditType == EditModeType.Insert)
            {   //新增
                RunInsertEnd HResult = this.acd.InsertMaster(mdd, LoginUserId);
                rAjaxResult = HandleResultAjaxFiles(HResult, Resources.Res.Data_Insert_Success);
                rAjaxResult.id = HResult.InsertId;
            }
            else
            {
                //修改
                RunEnd HResult = this.acd.UpdateMaster(mdd, LoginUserId);
                rAjaxResult = HandleResultAjaxFiles(HResult, Resources.Res.Data_Update_Success);
                rAjaxResult.id = mdd.ids;
            }
            return JsonConvert.SerializeObject(rAjaxResult, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
        }

        [HttpPost]
        public override String ajax_MasterDeleteData(String id)
        {
            String returnString = string.Empty;

            ReturnAjaxFiles rAjaxResult = new ReturnAjaxFiles();
            ac = new a_Schools() { Connection = getSQLConnection(), logPlamInfo = plamInfo }; ;

            RunDeleteEnd HResult = ac.DeleteMaster(id.Split(',').CInt(), LoginUserId);
            rAjaxResult = HandleResultAjaxFiles(HResult, Resources.Res.Data_Delete_Success);
            return JsonConvert.SerializeObject(rAjaxResult, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
        }

        [HttpGet]
        public override String ajax_MasterGridData(q_Schools queryObj)
        {
            #region 連接BusinessLogicLibary資料庫並取得資料
            ac = new a_Schools() { Connection = getSQLConnection(), logPlamInfo = plamInfo }; ;

            RunQueryPackage<m_Schools> HResult = ac.SearchMaster(queryObj, LoginUserId);
            HandleResultCheck(HResult);
            #endregion
            #region 設定 Page物件 頁數 總筆數 每頁筆數
            int page = (queryObj.page == null ? 1 : queryObj.page.CInt());
            int startRecord = PageCount.PageInfo(page, 30, HResult.Count);
            #endregion
            #region 每行及每個欄位資料組成
            List<RowElement> setRowElement = new List<RowElement>();
            var Modules = HResult.SearchData.Skip(startRecord).Take(30);
            foreach (m_Schools md in Modules)
            {
                List<String> setFields = new List<String>();

                setFields.Add(md.id.ToString());
                setFields.Add(md.kid.ToString());
                setFields.Add(md.name);
                setFields.Add(md.sort.ToString());
                setRowElement.Add(new RowElement() { id = md.id.ToString(), cell = setFields.ToArray() });
            }
            #endregion
            #region 回傳JSON資料

            return JsonConvert.SerializeObject(new JQGridDataObject()
            {
                rows = setRowElement.ToArray(),
                total = PageCount.TotalPage,
                page = PageCount.Page,
                records = PageCount.RecordCount
            }, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            #endregion
        }

        [HttpGet]
        public override String ajax_MasterSubGridData(q_Department queryObj)
        {
            #region 連接BusinessLogicLibary資料庫並取得資料
            acd = new a_Department() { Connection = getSQLConnection(), logPlamInfo = plamInfo }; ;

            RunQueryPackage<m_Department> HResult = acd.SearchMaster(queryObj, LoginUserId);
            HandleResultCheck(HResult);
            #endregion
            #region 設定 Page物件 頁數 總筆數 每頁筆數
            int page = (queryObj.page == null ? 1 : queryObj.page.CInt());
            int startRecord = PageCount.PageInfo(page, this.DefPageSize, HResult.Count);
            #endregion
            #region 每行及每個欄位資料組成
            List<RowElement> setRowElement = new List<RowElement>();
            var Modules = HResult.SearchData.Skip(startRecord).Take(this.DefPageSize);
            foreach (m_Department md in Modules)
            {
                List<String> setFields = new List<String>();
                setFields.Add(md.ids.ToString());
                setFields.Add(md.kid.ToString());
                setFields.Add(md.name);
                setFields.Add(md.zip);
                setFields.Add(md.sort.ToString());
                setRowElement.Add(new RowElement() { id = md.ids.ToString(), cell = setFields.ToArray() });
            }
            #endregion
            #region 回傳JSON資料
            JQGridDataObject dataObj = new JQGridDataObject()
            {
                rows = setRowElement.ToArray(),
                total = PageCount.TotalPage,
                page = PageCount.Page,
                records = PageCount.RecordCount
            };

            JavaScriptSerializer js = new JavaScriptSerializer() { MaxJsonLength = 65536 }; //64K
            return js.Serialize(dataObj);
            #endregion
        }

        [HttpPost]
        public override String ajax_SubDataDelete(String id)
        {
            JavaScriptSerializer js = new JavaScriptSerializer() { MaxJsonLength = 65536 }; //64K

            String returnString = string.Empty;

            ReturnAjaxFiles rAjaxResult = new ReturnAjaxFiles();
            acd = new a_Department() { Connection = getSQLConnection(), logPlamInfo = plamInfo }; ;
            RunDeleteEnd HResult = acd.DeleteMaster(id.Split(',').CInt(), LoginUserId);
            rAjaxResult = HandleResultAjaxFiles(HResult, Resources.Res.Data_Delete_Success);
            return js.Serialize(rAjaxResult);
        }

        [HttpPost]
        public override String ajax_SubDataUpdate(m_Department md)
        {
            ReturnAjaxFiles rAjaxResult = new ReturnAjaxFiles();
            String returnPicturePath = String.Empty;

            a_Department acd = new a_Department() { Connection = getSQLConnection(), logPlamInfo = plamInfo };
            acd.Connection = getSQLConnection();

            if (md.oper == "add")
            {   //新增
                RunInsertEnd HResult = acd.InsertMaster(md, LoginUserId);
                rAjaxResult = HandleResultAjaxFiles(HResult, Resources.Res.Data_Insert_Success);
                rAjaxResult.id = HResult.InsertId;
            }
            else
            {
                //修改
                RunEnd HResult = acd.UpdateMaster(md, LoginUserId);
                rAjaxResult = HandleResultAjaxFiles(HResult, Resources.Res.Data_Update_Success);
                rAjaxResult.id = md.ids;
            }
            JavaScriptSerializer js = new JavaScriptSerializer() { MaxJsonLength = 65536 }; //64K
            return js.Serialize(rAjaxResult);
        }
        public override String ajax_DetailGridData(q_Department ssh)
        {
            throw new NotImplementedException();
        }
        public override String ajax_DetailUpdata(m_Department md)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
