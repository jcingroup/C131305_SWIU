using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Resources;
using System.Runtime.Caching;
using System.Web.Mvc;
using System.Web.WebPages;
using System.Drawing;
using DotWeb.CommSetup;

using ProcCore.CKEdit;
using ProcCore.JqueryHelp;
using ProcCore.JqueryHelp.AjaxFilesUpLoadHelp;
using ProcCore.JqueryHelp.AjaxFormObj;
using ProcCore.JqueryHelp.AjaxHelp;
using ProcCore.JqueryHelp.AutocompleteHelp;
using ProcCore.JqueryHelp.CustomButton;
using ProcCore.JqueryHelp.DateTimePickerHelp;
using ProcCore.JqueryHelp.FormvValidate;
using ProcCore.JqueryHelp.JQGridScript;
using ProcCore.JqueryHelp.JSTreeHelp;
using ProcCore.JqueryHelp.DialogHelp;
using ProcCore.NetExtension;
using ProcCore.JqueryHelp.AddressHandle;
using ProcCore.DatabaseCore.TableFieldModule;

namespace DotWeb.Helpers
{
    public static class HtmlWebObject
    {
        #region JQuery Ui framework ICON section
        private static String ConvertIcornString(FrameworkIcons icons)
        {
            return icons.ToString().Replace("_", "-");
        }

        /// <summary>
        /// 資源檔圖檔協助器。
        /// </summary>
        public static MvcHtmlString GetImageFor(this HtmlHelper helper, Bitmap bitmap, string AltText = "")
        {
            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            stream.Seek(0, SeekOrigin.Begin);
            String base64 = Convert.ToBase64String(stream.ToArray());

            return new MvcHtmlString(String.Format("<img src='data:image/gif;base64,{0}' alt='{1}' />", base64, AltText));
        }

        public static MvcHtmlString ButtonScript(this HtmlHelper helper, jqButton button)
        {
            return MvcHtmlString.Create(button.ToScriptString());
        }
        public static MvcHtmlString FrameworkButton(this HtmlHelper helper, String Id, String text, FrameworkIcons icons)
        {
            jqButton button = new jqButton(new jqSelector() { IdName = Id });
            button.options.icons.primary = icons;
            return MvcHtmlString.Create("<button id=\"" + Id + "\" type=\"button\">" + text + "</button><script type=\"text/javascript\">" + button.ToScriptString() + "</script>");
        }

        public static MvcHtmlString GetFrameworkICON(this HtmlHelper helper, String Id, FrameworkIcons icons, String title)
        {
            String html = @"<span id=""{0}"" class=""ui-icon {1}"" title=""{2}"" ></span>";
            return MvcHtmlString.Create(String.Format(html, Id, icons.ToString().Replace("_", "-"), title));
        }
        public static MvcHtmlString FrameworkICON(this HtmlHelper helper, FrameworkIcons icons, String title)
        {
            String html = "<span class=\"ui-icon {0}\" title=\"{1}\" ></span>";
            return MvcHtmlString.Create(String.Format(html, icons.ToString().Replace("_", "-"), title));
        }

        public static MvcHtmlString GetNote(this HtmlHelper helper)
        {
            //String html = @"<span class=""note"" title=""{0}"" >＊</span>";
            String html = "<span class=\"ui-state-highlight ui-icon " + FrameworkIcons.ui_icon_star.ToString().Replace("_", "-") + "\" title=\"{0}\" ></span>";
            return MvcHtmlString.Create(String.Format(html, Resources.Res.Info_StarMustEdit));
        }
        public static MvcHtmlString SetMemo(this HtmlHelper helper, String Memo)
        {
            //String html = @"<span class=""note"" title=""{0}"" >＊</span>";
            String html = "<span class=\"ui-state-highlight ui-icon " + FrameworkIcons.ui_icon_help.ToString().Replace("_", "-") + "\" title=\"{0}\" ></span>";
            return MvcHtmlString.Create(String.Format(html, Memo));
        }

        #endregion

        #region JQuery Ui DateTimePicker Section
        public static MvcHtmlString DateTimePickerPlugin(this HtmlHelper h, jqSelector ElemntID, DateTimePicker option)
        {
            DateTimePickerUI jqObj = new DateTimePickerUI(ElemntID);
            if (option != null)
                jqObj.Options = option;

            return MvcHtmlString.Create(jqObj.ToScriptString());
        }

        /// <summary>
        /// 日期元件
        /// </summary>
        /// <param name="e">強型 代入欄位</param>
        /// <param name="option">參數選項設定</param>
        /// <returns>Jquery Script字串</returns>
        public static MvcHtmlString DateTimePickerPlugin<TModel, TProperty>(this HtmlHelper<TModel> h, Expression<Func<TModel, TProperty>> e, DateTimePicker option)
        where TModel : class
        {
            String inputName = ExpressionHelper.GetExpressionText(e);
            return DateTimePickerPlugin(h, new jqSelector() { IdName = inputName }, option);
        }
        #endregion

        #region JQuery Ui Autocomplete Section
        public static MvcHtmlString AutocompletePlugin(this HtmlHelper h, String ElemntID, AutocompleteHandle.Autocomplete option)
        {
            AutocompleteHandle jqObj = new AutocompleteHandle(new jqSelector() { IdName = ElemntID });
            if (option != null)
            {
                jqObj.Options = option;
                jqObj.Options.delay = 500;
            }
            return MvcHtmlString.Create(jqObj.ToScriptString());
        }
        public static MvcHtmlString AutocompletePlugin<TModel, TProperty>(this HtmlHelper<TModel> h, Expression<Func<TModel, TProperty>> e, AutocompleteHandle.Autocomplete option)
        where TModel : class
        {
            String n = ExpressionHelper.GetExpressionText(e);
            return AutocompletePlugin(h, n, option);
        }
        public static MvcHtmlString AutocompleteSimple<TModel, TProperty>(this HtmlHelper<TModel> h, Expression<Func<TModel, TProperty>> e, String[] data)
        where TModel : class
        {
            String n = ExpressionHelper.GetExpressionText(e);
            return AutocompletePlugin(h, n, new AutocompleteHandle.Autocomplete() { source = new MutileType() { attrType = MutileType.AttrType.StringArray, StringArray = data } });
        }

        /// <summary>
        /// 回傳給Key值固定變數為 val 例：action?val=value
        /// </summary>
        public static MvcHtmlString mhs_AutocompleteAjax<TModel, TProperty>(this HtmlHelper<TModel> h, Expression<Func<TModel, TProperty>> e, String Url)
        where TModel : class
        {
            String n = ExpressionHelper.GetExpressionText(e);
            ajaxObject ajax = new ajaxObject();
            ajax.url = Url;
            ajax.type = "get";
            ajax.success = new funcMethodModule() { MakeStyle = funcMethodModule.funcMakeStyle.funcConext, funcString = "response(data)" };
            ajax.data = new DataModule();
            ajax.data.Add("val", "$('#" + n + "').val()");

            return AutocompletePlugin(h, n, new AutocompleteHandle.Autocomplete()
            {
                source = new MutileType()
                {
                    attrType = MutileType.AttrType.funcMethod,
                    funcMethod = new funcMethodModule()
                    {
                        MakeStyle = funcMethodModule.funcMakeStyle.funcConext,
                        parmsRange = new String[] { "request", "response" },
                        funcString = ajax.ToSelfScriptString()
                    }
                }
            });
        }

        /// <summary>
        /// 回傳給Key值固定變數為 val 例：action?val=value
        /// </summary>
        public static AutocompleteHandle AutocompleteAjax<TModel, TProperty>(this HtmlHelper<TModel> h, Expression<Func<TModel, TProperty>> e, String Url)
        where TModel : class
        {
            String n = ExpressionHelper.GetExpressionText(e);
            ajaxObject ajax = new ajaxObject();
            ajax.url = Url;
            ajax.type = "get";
            ajax.success = new funcMethodModule() { MakeStyle = funcMethodModule.funcMakeStyle.funcConext, funcString = "response(data)" };
            ajax.data = new DataModule();
            ajax.data.Add("val", "$('#" + n + "').val()");

            var option = new AutocompleteHandle.Autocomplete()
            {
                source = new MutileType()
                {
                    attrType = MutileType.AttrType.funcMethod,
                    funcMethod = new funcMethodModule()
                    {
                        MakeStyle = funcMethodModule.funcMakeStyle.funcConext,
                        parmsRange = new String[] { "request", "response" },
                        funcString = ajax.ToSelfScriptString()
                    }
                }
            };

            AutocompleteHandle jqObj = new AutocompleteHandle(new jqSelector() { IdName = n });

            jqObj.Options = option;
            jqObj.Options.delay = 500;
            return jqObj;
        }

        #endregion

        #region AjaxForm and Rules Setup section
        public static MvcHtmlString AjaxFormSearch(this HtmlHelper helper, String FormId, String GridId, String SubmitEleName, Boolean UseDefault)
        {
            AjaxFormObject ajaxForm = new AjaxFormObject(FormId);
            if (UseDefault)
            {
                ajaxForm.SubmitElementId = SubmitEleName;
                ajaxForm.SubmitEvent = HtmlObjectEvent.click;
                ajaxForm.options = new FormAjaxOption();

                ajaxForm.options.beforeSubmit = new funcMethodModule() { MakeStyle = funcMethodModule.funcMakeStyle.funcConext };
                ajaxForm.options.success = new funcMethodModule() { MakeStyle = funcMethodModule.funcMakeStyle.funcConext };

                ajaxForm.options.dataType = "json";
                ajaxForm.options.beforeSubmit.funcString = "var queryString = $.param(formData);return true;";

                if (GridId != null)
                {
                    ajaxForm.options.success.funcString = @"
                        var GetGridMaster = jQuery('#" + GridId + @"')[0];
                        GetGridMaster.addJSONData(jsonobj);
                        //jQuery('#" + GridId + @"').editGridRow( 24 );
                        if(jsonobj.records==0){jsonobj.message = '" + Resources.Res.Search_No_Data + "';$.UiMessage(jsonobj);GetGridMaster=null;jsonobj=null}";

                }
            }

            TagBuilder script = new TagBuilder("script");
            script.MergeAttribute("type", "text/javascript");
            script.InnerHtml = ajaxForm.ToScriptString();
            return MvcHtmlString.Create(script.ToString());
        }
        public static MvcHtmlString AjaxFormPrint(this HtmlHelper helper, String FormId, String GridId, String SubmitEleName, Boolean UseDefault)
        {
            AjaxFormObject ajaxForm = new AjaxFormObject(FormId);
            if (UseDefault)
            {
                ajaxForm.SubmitElementId = SubmitEleName;
                ajaxForm.SubmitEvent = HtmlObjectEvent.click;
                ajaxForm.options = new FormAjaxOption();

                ajaxForm.options.beforeSubmit = new funcMethodModule() { MakeStyle = funcMethodModule.funcMakeStyle.funcConext };
                ajaxForm.options.success = new funcMethodModule() { MakeStyle = funcMethodModule.funcMakeStyle.funcConext };

                ajaxForm.options.dataType = "json";
                ajaxForm.options.beforeSubmit.funcString = "var queryString = $.param(formData);return true;";

                if (GridId != null)
                {
                    ajaxForm.options.success.funcString = @"
                        var GetGridMaster = jQuery('#" + GridId + @"')[0];
                        GetGridMaster.addJSONData(jsonobj);
                        //jQuery('#" + GridId + @"').editGridRow( 24 );
                        if(jsonobj.records==0){jsonobj.message = '" + Resources.Res.Search_No_Data + "';$.UiMessage(jsonobj);GetGridMaster=null;jsonobj=null}";

                }
            }

            TagBuilder script = new TagBuilder("script");
            script.MergeAttribute("type", "text/javascript");
            script.InnerHtml = ajaxForm.ToScriptString();
            return MvcHtmlString.Create(script.ToString());
        }

        public static FormValidateModels FieldsRuleSetup<TModel, TProperty>(this HtmlHelper<TModel> h,
            Expression<Func<TModel, TProperty>> e,
             FieldRule r, FieldMessage m, Dictionary<String, String> xr)
        {
            return FieldsRuleSetup(h, ExpressionHelper.GetExpressionText(e), r, m, xr);
        }

        public static FormValidateModels FieldsRuleSetup(this HtmlHelper h, String f, FieldRule r, FieldMessage m, Dictionary<String, String> xr)
        {
            FormValidateModels o = new FormValidateModels() { fieldName = f };
            if (r != null)
                o.rules = r;
            if (m != null)
                o.messages = m;
            if (xr != null)
                o.ExtraRule = xr;

            return o;
        }

        public static funcMethodModule CommSetFormOnSuccesss(this HtmlHelper h)
        {
            funcMethodModule func = new funcMethodModule() { funcName = "$.FormResultJson", MakeStyle = funcMethodModule.funcMakeStyle.jqfunc };
            func.funcParam.Add("response");
            func.funcString = @"
    var jsonobj = jQuery.parseJSON(response);
    if (jsonobj.result)
        $('#EditType').val('Modify');
    $.UiMessage(jsonobj);";

            return func;
        }
        public static funcMethodModule CommSetFormOnSearch(this HtmlHelper h)
        {
            funcMethodModule func = new funcMethodModule() { funcName = "$.FormResultSearch", MakeStyle = funcMethodModule.funcMakeStyle.jqfunc };
            func.funcParam.Add("response");
            func.funcString = @"
    var jsonobj = jQuery.parseJSON(response);
    if (jsonobj.result){

        }
    $.UiMessage(jsonobj);";
            return func;
        }



        public static String FM_BFR_Hdl_CKEdit(this HtmlHelper h)
        {
            return "for ( instance in CKEDITOR.instances ) CKEDITOR.instances[instance].updateElement();";
        }


        public static MvcHtmlString OpenDialogPlugin(this HtmlHelper helper, String Title, String Message)
        {
            if (Message != "")
            {
                String s = "$('#dialog').html('" + Message + @"');"
                        + "$('#dialog').dialog({ title: '" + Title + "'})";

                TagBuilder script = new TagBuilder("script");
                script.MergeAttribute("type", "text/javascript");
                script.InnerHtml = (s + ";").ToJqueryDocumentReady();
                return MvcHtmlString.Create(script.ToString());
            }
            else
            {
                return MvcHtmlString.Create("");
            }
        }
        #endregion

        #region Tree Handle
        public static MvcHtmlString TreeHelp(this HtmlHelper helper, String Id)
        {
            JSTreeHandle jt = new JSTreeHandle(Id);
            jt.jsTree = new JSTree();
            jt.jsTree.plugins = new Plugins[] { Plugins.themes, Plugins.json_data, Plugins.ui };
            jt.jsTree.json_data = new Json_Data();
            jt.jsTree.json_data.ajax = new ProcCore.JqueryHelp.AjaxHelp.ajaxObject();
            jt.jsTree.json_data.ajax.url = "ajax_Tree";
            jt.jsTree.json_data.ajax.dataType = "json";
            jt.jsTree.json_data.ajax.contentType = null;
            jt.jsTree.json_data.ajax.type = "get";
            jt.jsTree.json_data.ajax.async = true;

            jt.jsTree.json_data.ajax.data_Func = new funcMethodModule();
            jt.jsTree.json_data.ajax.data_Func.parmsRange = new String[] { "n" };
            jt.jsTree.json_data.ajax.data_Func.funcString = "return { id : n.attr ? n.attr(\"id\") : 0 }; ";
            jt.jsTree.json_data.ajax.data_Func.MakeStyle = funcMethodModule.funcMakeStyle.funcConext;

            jt.jsTree.json_data.ajax.error = new funcMethodModule();
            jt.jsTree.json_data.ajax.error.MakeStyle = funcMethodModule.funcMakeStyle.funcConext;
            jt.jsTree.json_data.ajax.error.funcString = "alert(errorThrown)";

            jt.jsBind = new List<JSBind>() { new JSBind() { JsEvent = JSBindEvent.select_node, FunctionString = "var nodeId = data.rslt.obj.data(\"id\");$(\"#productkind\").val(nodeId);Call_Ajax_A(nodeId);" } };

            return MvcHtmlString.Create(jt.ToScriptString());
        }

        #endregion

        public static String SetCommCKEditor2(this HtmlHelper helper, String Id, Boolean UseCKFinder)
        {
            CKEditor ck = new CKEditor(new jqSelector() { IdName = Id });

            ck.Options.height = 320;
            ck.Options.toolbar = new CKEditor.Toolbar[]{
                new CKEditor.Toolbar(){name=EditBarNames.basicstyles,items=new EditFun[]{EditFun.FontSize, EditFun.Bold, EditFun.Italic, EditFun.Strike, EditFun.Dot_, EditFun.RemoveFormat}},
                new CKEditor.Toolbar(){name=EditBarNames.paragraph,items=new EditFun[]{EditFun.NumberedList, EditFun.BulletedList, EditFun.Dot_, EditFun.Outdent, EditFun.Indent, EditFun.Dot_, EditFun.Blockquote}},
                new CKEditor.Toolbar(){name=EditBarNames.tools,items=new EditFun[]{EditFun.Maximize, EditFun.Dot_, EditFun.Image,EditFun.Table}},
                new CKEditor.Toolbar(){name=EditBarNames.styles,items=new EditFun[]{EditFun.Styles, EditFun.Format}},
                new CKEditor.Toolbar(){name=EditBarNames.links,items=new EditFun[]{ EditFun.Link, EditFun.Unlink, EditFun.Anchor}},
                new CKEditor.Toolbar(){name=EditBarNames.colors,items=new EditFun[]{ EditFun.TextColor, EditFun.BGColor}},
                new CKEditor.Toolbar(){name=EditBarNames.editing,items=new EditFun[]{ EditFun.Find, EditFun.Replace, EditFun.SelectAll}},
                new CKEditor.Toolbar(){name=EditBarNames.document,items=new EditFun[]{ EditFun.Source, EditFun.Dot_, EditFun.DocProps}},
                new CKEditor.Toolbar(){name=EditBarNames.clipboard,items=new EditFun[]{ EditFun.Cut, EditFun.Copy, EditFun.Paste, EditFun.PasteText, EditFun.PasteFromWord, EditFun.Undo, EditFun.Redo}}
            };

            if (UseCKFinder)
            {
                ck.Options.filebrowserBrowseUrl = "../../_Code/ckfinder/ckfinder.html";
                ck.Options.filebrowserImageBrowseUrl = "../../_Code/ckfinder/ckfinder.html?type=Images";
                ck.Options.filebrowserFlashBrowseUrl = "../../_Code/ckfinder/ckfinder.html?type=Flash";
                ck.Options.filebrowserUploadUrl = "../../_Code/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=Files";
                ck.Options.filebrowserImageUploadUrl = "../../_Code/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=Images";
                ck.Options.filebrowserFlashUploadUrl = "../../_Code/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=Flash";
                ck.Options.filebrowserWindowWidth = 1000;
                ck.Options.filebrowserWindowHeight = 700;
            }
            return ck.ToScriptString();
        }


        public static String AddressScript(this HtmlHelper helper, String ZipId, String CityId, String CountyId, String CityValue, String CountyValue)
        {
            AddressAjaxHandle A = new AddressAjaxHandle(CityId);
            A.options.countyElement = new jqSelector() { IdName = CountyId };
            A.options.zipElement = new jqSelector() { IdName = ZipId };
            A.options.cityValue = CityValue;
            A.options.countyValue = CountyValue;
            return A.ToScriptString();
        }

        /// <summary>
        ///  &lt;label for=""&gt;&lt;span class=""&gt;本文&lt;/span&gt;&lt;/label&gt;
        /// </summary>
        /// <returns></returns>
        public static MvcHtmlString LabelField<TModel, TProperty>(this HtmlHelper<TModel> h, Expression<Func<TModel, TProperty>> e, String t)
        {
            return LabelField(h, ExpressionHelper.GetExpressionText(e), t);
        }
        public static MvcHtmlString LabelField(this HtmlHelper h, String n, String t)
        {
            return MvcHtmlString.Create(String.Format("<label for=\"{0}\">{1}</label>", n, t));
        }

        public static MvcHtmlString LabelHiddenField<TModel, TProperty>(this HtmlHelper<TModel> h, Expression<Func<TModel, TProperty>> e, String t)
        {
            return LabelHiddenField(h, ExpressionHelper.GetExpressionText(e), t);
        }
        public static MvcHtmlString LabelHiddenField(this HtmlHelper h, String n, String t)
        {
            return MvcHtmlString.Create(String.Format("<span>{1}</span><input type=\"hidden\" name=\"{0}\" id=\"{0}\" value=\"{1}\">", n, t));
        }

        /// <summary>
        /// 屬性及取值範例
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="h"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static MvcHtmlString SpanText<TModel, TProperty>(this HtmlHelper<TModel> h, Expression<Func<TModel, TProperty>> e)
        {
            String n = ExpressionHelper.GetExpressionText(e);
            var v = h.ViewData.Model.GetType().GetProperty(n).GetValue(h.ViewData.Model, null);
            return MvcHtmlString.Create(String.Format("<span id=\"{0}\">{1}</span>", n, v));
        }
        public static MvcHtmlString SpanText<TModel, TProperty>(this HtmlHelper<TModel> h, Expression<Func<TModel, TProperty>> e,String v)
        {
            String n = ExpressionHelper.GetExpressionText(e);
            return MvcHtmlString.Create(String.Format("<span id=\"{0}\">{1}</span>", n, v));
        }

        public static MvcHtmlString TextHiddenField<TModel, TProperty>(this HtmlHelper<TModel> h, Expression<Func<TModel, TProperty>> e, String v, String t)
        {
            return TextHiddenField(h, ExpressionHelper.GetExpressionText(e), v, t);
        }
        public static MvcHtmlString TextHiddenField(this HtmlHelper h, String n, String v, String t)
        {
            return MvcHtmlString.Create(String.Format("<input type=\"text\" id=\"ex_{0}\" name=\"ex_{0}\" value=\"{2}\"><input type=\"hidden\" name=\"{0}\" id=\"{0}\" value=\"{1}\">", n, v, t));
        }

    }
    public static class jqGridHelper
    {
        #region UI jqGrid Section
        private static String ConvertIcornString(FrameworkIcons icons)
        {
            return icons.ToString().Replace("_", "-");
        }
        #region Grid Help 2.0

        public static jqGrid jqGridMobile_Standard(this HtmlHelper helper, String Id, String Caption, String Page,
    String AddQuery, String DataUrl, String DelUrl, String AddUrl, int Height, jqGrid subGrid, params MakeColumnModule[] GridColumnSetup)
        {
            return jqGridMobile_Standard(helper, Id, Caption, Page, AddQuery, DataUrl, DelUrl, AddUrl, Height, 10, subGrid, GridColumnSetup);
        }

        public static jqGrid jqGridMobile_Standard(this HtmlHelper helper, String Id, String Caption, String Page,
    String AddQuery, String DataUrl, String DelUrl, String AddUrl, int Height, int rowNum, jqGrid subGrid, params MakeColumnModule[] GridColumnSetup)
        {
            //新增按鍵 路徑
            String f = @"var getQuery = $.CollectQuery();document.location.href = '{0}?' + $.pageQuery('" + Id + @"') + (getQuery == '' ? '' : '&' + getQuery);";
            f = String.Format(f, AddUrl);

            List<String> colNames = new List<String>();
            List<jqGrid.colObject> colModel = new List<jqGrid.colObject>();
            //處理Column設定
            foreach (var c in GridColumnSetup)
            {
                if (c.CM.AssignFormatter != null)
                {
                    if (c.CM.formatter == null) c.CM.formatter = new funcMethodModule() { MakeStyle = funcMethodModule.funcMakeStyle.onlyName };
                    c.CM.formatter.funcName = c.CM.AssignFormatter.func.funcName;
                }
                colNames.Add(c.CN);
                colModel.Add(c.CM);
            }

            jqGrid jd = new jqGrid()
            {
                Id = Id,
                GridModule = new jqGrid.gridMasterObject()
                {
                    colModel = colModel.ToArray(),
                    colNames = colNames.ToArray(),
                    caption = Caption,
                    pager = "pager",
                    url = DataUrl + (AddQuery == "" ? "" : "?") + AddQuery,
                    page = Page == null ? 1 : int.Parse(Page),
                    autowidth = true,
                    multiboxonly = false,
                    multiselect = false,
                    height = Height,
                    scroll = false,
                    search = true,
                    gridview = true,
                    viewrecords = true,
                    hoverrows = false,
                    rowNum = rowNum,
                    loadError = new funcMethodModule()
                    {
                        funcString = @"
						try {
							jQuery.jgrid.info_dialog(jQuery.jgrid.errors.errcap,'<div class=""ui-state-error"">'+ xhr.responseText +'</div>', jQuery.jgrid.edit.bClose,
                            {buttonalign:'right'});	} catch(e) {alert(xhr.responseText);}",
                        MakeStyle = funcMethodModule.funcMakeStyle.funcConext
                    },
                    gridComplete = new funcMethodModule()
                    {
                        funcString = "$('.divGridFunctionButton').trigger('create');",
                        MakeStyle = funcMethodModule.funcMakeStyle.funcConext
                    }
                },
                NavGridModule = new jqGrid.navGridObject()
                {
                    navOption = new jqGrid.navGridObject.navGridOption()
                    {
                        alerttext = Resources.Res.Select_Delete_Data,
                        del = true,
                        deltitle = Resources.Res.Info_Delete_Data,
                        deltext = Resources.Res.Button_Delete,
                        refresh = true,
                        add = false,
                        edit = false,
                        search = true
                    },
                    Del = new jqGrid.navGridObject.editGridRow() { url = DelUrl, msg = Resources.Res.IsSureDelete },
                    Add = new jqGrid.navGridObject.editGridRow(),
                    Edit = new jqGrid.navGridObject.editGridRow()
                }
                ,
                navCustomButtons =
                    new jqGrid.navButtonAddModule[] { 
                    new jqGrid.navButtonAddModule {    caption = Resources.Res.Button_Insert, 
                                                                    buttonicon = "plus",
                                                                    onClickButton=new funcMethodModule{funcString=f, MakeStyle= funcMethodModule.funcMakeStyle.funcConext},
                                                                    position="last" ,
                                                                }
                }
            };

            if (subGrid != null)
            {
                jd.GridModule.subGrid = true;
                jd.GridModule.ExpandColClick = true;
                jd.GridModule.subGridRowExpanded = new funcMethodModule() { MakeStyle = funcMethodModule.funcMakeStyle.funcConext };
                jd.GridModule.subGridRowExpanded.funcString = "var subgrid_table_id;var subgrid_pager_id;subgrid_table_id = pID+ '_t';subgrid_pager_id = pID + '_p';"
  + "jQuery(\"#\"+pID).html(\"<table id='\"+subgrid_table_id+\"' class='scroll'></table><div id='\" + subgrid_pager_id + \"'></div>\");";
                subGrid.ToScriptHandle();
                jd.GridModule.subGridRowExpanded.funcString += subGrid.jqGridScript;
            }
            return jd;
        }
        /// <summary>
        /// 標準式Grid，導入客制編輯畫面，新增及修改Button為特殊設定
        /// </summary>
        public static jqGrid jqGrid_Standard(this HtmlHelper helper, String Id, String Caption, String Page,
    String AddQuery, String DataUrl, String DelUrl, String AddUrl, int Height, jqGrid subGrid, params MakeColumnModule[] GridColumnSetup)
        {
            return jqGrid_Standard(helper, Id, Caption, Page, AddQuery, DataUrl, DelUrl, AddUrl, Height, 10, subGrid, GridColumnSetup);
        }

        public static jqGrid jqGrid_Standard(this HtmlHelper helper, String Id, String Caption, String Page,
    String AddQuery, String DataUrl, String DelUrl, String AddUrl, int Height, int rowNum, jqGrid subGrid, params MakeColumnModule[] GridColumnSetup)
        {
            //新增按鍵 路徑
            String f = @"var getQuery = $.CollectQuery();document.location.href = '{0}?' + $.pageQuery('" + Id + @"') + (getQuery == '' ? '' : '&' + getQuery);";
            f = String.Format(f, AddUrl);

            List<String> colNames = new List<String>();
            List<jqGrid.colObject> colModel = new List<jqGrid.colObject>();
            //處理Column設定
            foreach (var c in GridColumnSetup)
            {
                if (c.CM.AssignFormatter != null)
                {
                    if (c.CM.formatter == null) c.CM.formatter = new funcMethodModule() { MakeStyle = funcMethodModule.funcMakeStyle.onlyName };
                    c.CM.formatter.funcName = c.CM.AssignFormatter.func.funcName;
                }
                colNames.Add(c.CN);
                colModel.Add(c.CM);
            }

            jqGrid jd = new jqGrid()
            {
                Id = Id,
                GridModule = new jqGrid.gridMasterObject()
                {
                    colModel = colModel.ToArray(),
                    colNames = colNames.ToArray(),
                    caption = Caption,
                    pager = "pager",
                    url = DataUrl + (AddQuery == "" ? "" : "?") + AddQuery,
                    page = Page == null ? 1 : int.Parse(Page),
                    autowidth = true,
                    multiboxonly = true,
                    height = Height,
                    rowNum = rowNum,
                    viewrecords = true
                },
                NavGridModule = new jqGrid.navGridObject()
                {
                    navOption = new jqGrid.navGridObject.navGridOption()
                    {
                        alerttext = Resources.Res.Select_Delete_Data,
                        del = true,
                        deltitle = Resources.Res.Info_Delete_Data,
                        deltext = Resources.Res.Button_Delete,
                        refresh = true,
                        add = false,
                        edit = false,
                        search = false
                    },
                    Del = new jqGrid.navGridObject.editGridRow() { url = DelUrl, msg = Resources.Res.IsSureDelete },
                    Add = new jqGrid.navGridObject.editGridRow(),
                    Edit = new jqGrid.navGridObject.editGridRow()
                },
                navCustomButtons =
                    new jqGrid.navButtonAddModule[] { 
                    new jqGrid.navButtonAddModule {    caption = Resources.Res.Button_Insert, 
                                                                    buttonicon = "ui-icon " + ConvertIcornString(FrameworkIcons.ui_icon_plusthick),
                                                                    onClickButton=new funcMethodModule{funcString=f, MakeStyle= funcMethodModule.funcMakeStyle.funcConext},
                                                                    position="last" ,
                                                                }
                }
            };

            if (subGrid != null)
            {
                jd.GridModule.subGrid = true;
                jd.GridModule.ExpandColClick = true;
                jd.GridModule.subGridRowExpanded = new funcMethodModule() { MakeStyle = funcMethodModule.funcMakeStyle.funcConext };
                jd.GridModule.subGridRowExpanded.funcString = "var subgrid_table_id;var subgrid_pager_id;subgrid_table_id = pID+ '_t';subgrid_pager_id = pID + '_p';"
  + "jQuery(\"#\"+pID).html(\"<table id='\"+subgrid_table_id+\"' class='scroll'></table><div id='\" + subgrid_pager_id + \"'></div>\");";
                subGrid.ToScriptHandle();
                jd.GridModule.subGridRowExpanded.funcString += subGrid.jqGridScript;
            }
            return jd;
        }


        public static MvcHtmlString mhs_jqGrid_Standard(this HtmlHelper helper, String Id, String Caption, String Page,
    String AddQuery, String DataUrl, String DelUrl, String AddUrl, int Height, jqGrid subGrid, params MakeColumnModule[] GridColumnSetup)
        {
            return MvcHtmlString.Create(jqGrid_Standard(helper, Id, Caption, Page, AddQuery, DataUrl, DelUrl, AddUrl, Height, 10, subGrid, GridColumnSetup).ToScriptString());
        }

        public static MvcHtmlString mhs_jqGrid_Standard(this HtmlHelper helper, String Id, String Caption, String Page,
    String AddQuery, String DataUrl, String DelUrl, String AddUrl, int Height, int rowNum, jqGrid subGrid, params MakeColumnModule[] GridColumnSetup)
        {
            return MvcHtmlString.Create(jqGrid_Standard(helper, Id, Caption, Page, AddQuery, DataUrl, DelUrl, AddUrl, Height, rowNum, subGrid, GridColumnSetup).ToScriptString());
        }


        /// <summary>
        /// 採用jqGrid編輯功能，不需導入Edit頁面。
        /// </summary>
        public static jqGrid jqGrid_Edit(this HtmlHelper helper, String Id, String Caption, String Page,
     String DataUrl, String DelUrl, String UpdateUrl, int Height, jqGrid subGrid, params MakeColumnModule[] GridColumnSetup)
        {

            String f = @"   var getQuery = $.CollectQuery();document.location.href = '{0}?' + $.pageQuery('" + Id + @"') + (getQuery == '' ? '' : '&' + getQuery);";

            List<String> colNames = new List<String>();
            List<jqGrid.colObject> colModel = new List<jqGrid.colObject>();
            //處理Column設定
            foreach (var c in GridColumnSetup)
            {
                if (c.CM.AssignFormatter != null)
                {
                    if (c.CM.formatter == null) c.CM.formatter = new funcMethodModule() { MakeStyle = funcMethodModule.funcMakeStyle.onlyName };
                    c.CM.formatter.funcName = c.CM.AssignFormatter.func.funcName;
                }
                colNames.Add(c.CN);
                colModel.Add(c.CM);
            }

            jqGrid jd = new jqGrid()
            {
                Id = Id,
                GridModule = new jqGrid.gridMasterObject()
                {
                    caption = Caption,
                    pager = "pager_" + Id,
                    url = DataUrl,
                    page = Page == null ? 1 : int.Parse(Page),
                    autowidth = true,
                    height = Height > 0 ? Height : 320,
                    multiselect = true,
                    multiboxonly = true,
                    colModel = colModel.ToArray(),
                    colNames = colNames.ToArray()
                },
                NavGridModule = new jqGrid.navGridObject()
                {
                    navOption = new jqGrid.navGridObject.navGridOption()
                    {
                        del = true,
                        deltitle = Resources.Res.Info_Delete_Data,
                        deltext = Resources.Res.Button_Delete,
                        refresh = true,
                        add = true,
                        addtext = Resources.Res.Button_Insert,
                        edit = true,
                        edittext = Resources.Res.Button_Modify,
                        search = false,
                    },
                    Add = new jqGrid.navGridObject.editGridRow() { url = UpdateUrl, closeAfterAdd = true },
                    Edit = new jqGrid.navGridObject.editGridRow() { url = UpdateUrl },
                    Del = new jqGrid.navGridObject.editGridRow() { url = DelUrl, msg = Resources.Res.IsSureDelete }
                }
            };

            if (subGrid != null)
            {
                jd.GridModule.subGrid = true;
                jd.GridModule.ExpandColClick = true;
                jd.GridModule.subGridRowExpanded = new funcMethodModule() { MakeStyle = funcMethodModule.funcMakeStyle.funcConext };
                jd.GridModule.subGridRowExpanded.funcString = "var subgrid_table_id;var subgrid_pager_id;subgrid_table_id = pID+ '_t';subgrid_pager_id = pID + '_p';"
  + "jQuery(\"#\"+pID).html(\"<table id='\"+subgrid_table_id+\"' class='scroll'></table><div id='\" + subgrid_pager_id + \"'></div>\");";
                subGrid.ToScriptHandle();
                jd.GridModule.subGridRowExpanded.funcString += subGrid.jqGridScript;
            }

            return jd;
        }

        /// <summary>
        /// 採用jqGrid編輯功能，不需導入Edit頁面。
        /// </summary>
        public static MvcHtmlString mhs_jqGrid_Edit(this HtmlHelper helper, String Id, String Caption, String Page,
     String DataUrl, String DelUrl, String UpdateUrl, int Height, jqGrid subGrid, params MakeColumnModule[] GridColumnSetup)
        {
            return MvcHtmlString.Create(jqGrid_Edit(helper, Id, Caption, Page, DataUrl, DelUrl, UpdateUrl, Height, subGrid, GridColumnSetup).ToScriptString());
        }

        /// <summary>
        /// SubGrid的Id 由主Grid提供
        /// </summary>
        public static jqGrid jqSubGrid(this HtmlHelper helper, String DataUrl, String DeleteActionUrl, String AddNewEditUrl, int? Height,
            jqGrid subGrid, params MakeColumnModule[] GridColumnSetup)
        {

            List<String> colNames = new List<String>();
            List<jqGrid.colObject> colModel = new List<jqGrid.colObject>();

            //處理Column設定
            foreach (var col in GridColumnSetup)
            {
                if (col.CM.AssignFormatter != null)
                {
                    if (col.CM.formatter == null) col.CM.formatter = new funcMethodModule() { MakeStyle = funcMethodModule.funcMakeStyle.onlyName };
                    col.CM.formatter.funcName = col.CM.AssignFormatter.func.funcName;
                }
                colNames.Add(col.CN);
                colModel.Add(col.CM);
            }

            jqGrid jd = new jqGrid()
            {
                GridIdWorkTosubGridRowExpanded = true,
                GridModule = new jqGrid.gridMasterObject()
                {
                    url = DataUrl,
                    multiboxonly = true,
                    postData = new DataModule(),
                    colModel = colModel.ToArray(),
                    colNames = colNames.ToArray(),
                    height = Height
                },
                NavGridModule = new jqGrid.navGridObject()
                {
                    navOption = new jqGrid.navGridObject.navGridOption() { del = true, edit = false, add = false, search = false, refresh = false, deltitle = Resources.Res.Info_Delete_Data, deltext = Resources.Res.Button_Delete },
                    Del = new jqGrid.navGridObject.editGridRow() { url = DeleteActionUrl, msg = Resources.Res.IsSureDelete },
                    Add = new jqGrid.navGridObject.editGridRow() { url = AddNewEditUrl },
                    Edit = new jqGrid.navGridObject.editGridRow() { url = AddNewEditUrl }
                }
            };

            if (subGrid != null)
            {
                jd.GridModule.subGrid = true;
                jd.GridModule.ExpandColClick = true;
                jd.GridModule.subGridRowExpanded = new funcMethodModule() { MakeStyle = funcMethodModule.funcMakeStyle.funcConext };
                jd.GridModule.subGridRowExpanded.funcString = "var subgrid_table_id;var subgrid_pager_id;subgrid_table_id = pID+ '_t';subgrid_pager_id = pID + '_p';"
  + "jQuery(\"#\"+pID).html(\"<table id='\"+subgrid_table_id+\"' class='scroll'></table><div id='\" + subgrid_pager_id + \"'></div>\");";
                subGrid.ToScriptHandle();
                jd.GridModule.subGridRowExpanded.funcString += subGrid.jqGridScript;
            }

            return jd;
        }

        public static jqGrid jqSubGridStandard(this HtmlHelper helper, String DataUrl, String DeleteActionUrl, String AddNewEditUrl, int? Height,
            jqGrid subGrid, params MakeColumnModule[] GridColumnSetup)
        {

            List<String> colNames = new List<String>();
            List<jqGrid.colObject> colModel = new List<jqGrid.colObject>();

            //處理Column設定
            foreach (var col in GridColumnSetup)
            {
                if (col.CM.AssignFormatter != null)
                {
                    if (col.CM.formatter == null) col.CM.formatter = new funcMethodModule() { MakeStyle = funcMethodModule.funcMakeStyle.onlyName };
                    col.CM.formatter.funcName = col.CM.AssignFormatter.func.funcName;
                }
                colNames.Add(col.CN);
                colModel.Add(col.CM);
            }

            //新增按鍵 路徑
            String f = String.Format("document.location.href = '{0}/?sid=' + id;", AddNewEditUrl);
            jqGrid jd = new jqGrid()
            {
                GridIdWorkTosubGridRowExpanded = true,
                GridModule = new jqGrid.gridMasterObject()
                {
                    url = DataUrl,
                    multiboxonly = true,
                    postData = new DataModule(),
                    colModel = colModel.ToArray(),
                    colNames = colNames.ToArray(),
                    height = Height
                },
                NavGridModule = new jqGrid.navGridObject()
                {
                    navOption = new jqGrid.navGridObject.navGridOption() { del = true, edit = false, add = false, search = false, refresh = false, deltitle = Resources.Res.Info_Delete_Data, deltext = Resources.Res.Button_Delete },
                    Del = new jqGrid.navGridObject.editGridRow() { url = DeleteActionUrl, msg = Resources.Res.IsSureDelete },
                    Add = new jqGrid.navGridObject.editGridRow() { url = AddNewEditUrl },
                    Edit = new jqGrid.navGridObject.editGridRow() { url = AddNewEditUrl }
                },
                navCustomButtons =
    new jqGrid.navButtonAddModule[] { 
                    new jqGrid.navButtonAddModule {    caption = Resources.Res.Button_Insert, 
                                                                    buttonicon = "ui-icon " + ConvertIcornString(FrameworkIcons.ui_icon_plusthick),
                                                                    onClickButton=new funcMethodModule{funcString=f, MakeStyle= funcMethodModule.funcMakeStyle.funcConext},
                                                                    position="last" ,
                                                                }
                }
            };

            if (subGrid != null)
            {
                jd.GridModule.subGrid = true;
                jd.GridModule.ExpandColClick = true;
                jd.GridModule.subGridRowExpanded = new funcMethodModule() { MakeStyle = funcMethodModule.funcMakeStyle.funcConext };
                jd.GridModule.subGridRowExpanded.funcString = "var subgrid_table_id;var subgrid_pager_id;subgrid_table_id = pID+ '_t';subgrid_pager_id = pID + '_p';"
  + "jQuery(\"#\"+pID).html(\"<table id='\"+subgrid_table_id+\"' class='scroll'></table><div id='\" + subgrid_pager_id + \"'></div>\");";
                subGrid.ToScriptHandle();
                jd.GridModule.subGridRowExpanded.funcString += subGrid.jqGridScript;
            }

            return jd;
        }

        #endregion

        /// <summary>
        /// formatter要使用自訂function所要加的參數 "cellValue", "options", "rowObject"
        /// </summary>
        public static String GridIDColumnCommScriptContext(this HtmlHelper helper, String GridId, String LinkActionName)
        {
            //Script 的參數 "cellValue", "options", "rowObject"
            String tpl = "var cellHtml = '<a href=\"" + LinkActionName + "?Id=' + jQuery.trim(options.rowId) + '&' + $.pageQuery('" + GridId + "') + '&' + getQuery + '\">{0}</a>';";
            tpl = String.Format(tpl, "<div class=\"ui-state-default ui-corner-all\" style=\"height:24px;width:24px;margin:1px\"><span class=\"ui-icon " + ConvertIcornString(FrameworkIcons.ui_icon_pencil) + "\" style=\"margin:3px\"></span></div>");
            String s = "var getQuery = $.CollectQuery();" + tpl + "return cellHtml;";
            return s;
        }

        /// <summary>
        /// formatter要使用自訂function所要加的參數 "cellValue", "options", "rowObject"
        /// </summary>
        public static String GridIDColumnCommScriptContext(this HtmlHelper helper,  String LinkActionName)
        {
            //Script 的參數 "cellValue", "options", "rowObject"
            String tpl = "var cellHtml = '<a href=\"" + LinkActionName + "?Id=' + jQuery.trim(options.rowId) + '\">{0}</a>';";
            tpl = String.Format(tpl, "<div class=\"ui-state-default ui-corner-all\" style=\"height:24px;width:24px;margin:1px\"><span class=\"ui-icon " + ConvertIcornString(FrameworkIcons.ui_icon_pencil) + "\" style=\"margin:3px\"></span></div>");
            String s = "var getQuery = $.CollectQuery();" + tpl + "return cellHtml;";
            return s;
        }


        /// <summary>
        /// formatter要使用自訂function所要加的參數 "cellValue", "options", "rowObject"
        /// </summary>
        public static String MobileGridIDColumnCommScript(this HtmlHelper helper, String GridId, String LinkActionName)
        {
            //Script 的參數 "cellValue", "options", "rowObject"
            //String 修改樣版 = "var cellHtml = '<a data-role=\"button\" href=\"" + LinkActionName + "?Id=' + jQuery.trim(options.rowId) + '&' + $.pageQuery('" + GridId + "') + '&' + getQuery + '\">{0}</a>';";
            String 修改樣版 = "var cellHtml = '<div class=\"divGridFunctionButton\"><button type=\"button\" class=\"RedireModify\" action=\"" + LinkActionName + "\" sn=\"' + options.rowId + '\" page=\"' + $.pageQuery('" + GridId + "') + '\" query=\"' + getQuery + '\">修改</button></div>';";
            //修改樣版 = String.Format(修改樣版, "<span class=\"ui-icon " + ConvertIcornString(FrameworkIcons.ui_icon_pencil) + "\"></span>");
            修改樣版 = String.Format(修改樣版, "修改");
            String s = "var getQuery = $.CollectQuery();" + 修改樣版 + "return cellHtml;";
            return s;
        }

        /// <summary>
        /// 格式：setGridParam({postData: {s_title:$('#s_title').val()}});
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="h"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static MvcHtmlString setGridParam_postData<TModel, TProperty>(this HtmlHelper<TModel> h, Expression<Func<TModel, TProperty>> e)
        {
            String n = ExpressionHelper.GetExpressionText(e);
            return MvcHtmlString.Create("setGridParam({postData: {" + n + ":$('#" + n + "').val()}})");
        }

        public static String ls_To_Options(this HtmlHelper helper, List<SelectListItem> l)
        {
            List<String> n = new List<string>();
            foreach (SelectListItem s in l)
            {
                n.Add(s.Value + ":" + s.Text);
            }
            return n.ToArray().JoinArray(";");
        }

        #endregion
    }
    public static class jqValue
    {
        public static String Prefix { get; set; }
        public static String jqJsonObjectString { get; set; }
        /// <summary>
        /// 處理指定板型為：$('#{1}').val({2}.{0})
        /// jsonobj.data
        /// </summary>
        public static MvcHtmlString valJ<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression) where TModel : class
        {
            String inputName = ExpressionHelper.GetExpressionText(expression);
            String[] s = inputName.Split('.');

            if (jqJsonObjectString == null || jqJsonObjectString == "")
                jqJsonObjectString = "jsonobj.data";

            String tpl = "$('#{1}').val({2}.{0});";
            if (s.Length > 1)
                return MvcHtmlString.Create(String.Format(tpl, s[s.Length - 1], Prefix + s[s.Length - 1], jqJsonObjectString));
            else
                return MvcHtmlString.Create(String.Format(tpl, s[0], Prefix + s[0], jqJsonObjectString));

        }
        /// <summary>
        /// 處理指定板型為：$('#{1}').val($.parseMsJsonDate({2}.{0}))
        /// jsonobj.data
        /// </summary>
        public static MvcHtmlString valJD<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression) where TModel : class
        {
            String inputName = ExpressionHelper.GetExpressionText(expression);
            String[] s = inputName.Split('.');

            if (jqJsonObjectString == null || jqJsonObjectString == "")
                jqJsonObjectString = "jsonobj.data";

            String tpl = "$('#{1}').val($.parseMsJsonDate({2}.{0}));";
            if (s.Length > 1)
                return MvcHtmlString.Create(String.Format(tpl, s[s.Length - 1], Prefix + s[s.Length - 1], jqJsonObjectString));
            else
                return MvcHtmlString.Create(String.Format(tpl, s[0], Prefix + s[0], jqJsonObjectString));
        }
        public static MvcHtmlString valTD<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression) where TModel : class
        {
            String inputName = ExpressionHelper.GetExpressionText(expression);
            String[] s = inputName.Split('.');

            if (jqJsonObjectString == null || jqJsonObjectString == "")
                jqJsonObjectString = "jsonobj.data";

            String tpl = "$('#{0}{1}').text($.parseMsJsonDate({2}.{1}));$('#hid_{0}{1}').val($.parseMsJsonDate({2}.{1}));";
            if (s.Length > 1)
                return MvcHtmlString.Create(String.Format(tpl, Prefix, s[s.Length - 1], jqJsonObjectString));
            else
                return MvcHtmlString.Create(String.Format(tpl, Prefix, s[0], jqJsonObjectString));

        }
        public static MvcHtmlString valT<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression) where TModel : class
        {
            String inputName = ExpressionHelper.GetExpressionText(expression);
            String[] s = inputName.Split('.');

            if (jqJsonObjectString == null || jqJsonObjectString == "")
                jqJsonObjectString = "jsonobj.data";

            String tpl = "$('#{0}{1}').text({2}.{1});$('#hid_{0}{1}').val({2}.{1});";
            if (s.Length > 1)
                return MvcHtmlString.Create(String.Format(tpl, Prefix, s[s.Length - 1], jqJsonObjectString));
            else
                return MvcHtmlString.Create(String.Format(tpl, Prefix, s[0], jqJsonObjectString));

        }
        public static MvcHtmlString fdlLabel<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, Object value) where TModel : class
        {
            String inputName = ExpressionHelper.GetExpressionText(expression);
            String[] s = inputName.Split('.');

            String tpl = "<label id=\"{0}{1}\">{2}</label><input type=\"hidden\" id=\"hid_{0}{1}\" name=\"{1}\" value=\"{2}\">";
            if (s.Length > 1)
                return MvcHtmlString.Create(String.Format(tpl, Prefix, s[s.Length - 1], value == null ? "" : value.ToString()));
            else
                return MvcHtmlString.Create(String.Format(tpl, Prefix, s[0], value == null ? "" : value.ToString()));
        }

        /// <summary>
        /// reutrn $('#s').val()
        /// </summary>
        public static MvcHtmlString GVal<TModel, TProperty>(this HtmlHelper<TModel> h, Expression<Func<TModel, TProperty>> expression) where TModel : class
        {
            String n = ExpressionHelper.GetExpressionText(expression);
            return MvcHtmlString.Create("$('#" + n + "').val()");
        }
    }

    public static class SysFilesHelp
    {
        private static String SystemUpFilePathTpl = "~/_Code/SysUpFiles/{0}.{1}/{2}/{3}/{4}";
        private static String SystemUpImagePathTpl = "~/_Code/SysUpFiles/{0}.{1}/{2}/{3}/{4}";

        #region File List Helper
        public static String[] FilesAppendInfo(this HtmlHelper h, String Area, String Controller, Int32 Id)
        {
            String SearchPath = String.Format(SystemUpFilePathTpl, Area, Controller, Id, "DocFiles", "OriginFile");
            String FolderPth = h.ViewContext.HttpContext.Server.MapPath(SearchPath);

            List<String> fiInfo = new List<String>();

            if (Directory.Exists(FolderPth))
            {
                String[] fs = Directory.GetFiles(FolderPth);

                foreach (String f in fs)
                {
                    FileInfo fi = new FileInfo(f);
                    fiInfo.Add(fi.Name);
                }
            }
            return fiInfo.ToArray();
        }
        public static String[] FilesAppendInfo(this HtmlHelper h, String Area, String Controller, Int32 Id, String FilesKind)
        {
            String SearchPath = String.Format(SystemUpFilePathTpl, Area, Controller, Id, FilesKind, "OriginFile");
            String FolderPth = h.ViewContext.HttpContext.Server.MapPath(SearchPath);

            List<String> fiInfo = new List<String>();

            if (Directory.Exists(FolderPth))
            {
                String[] fs = Directory.GetFiles(FolderPth);

                foreach (String f in fs)
                {
                    FileInfo fi = new FileInfo(f);
                    fiInfo.Add(fi.Name);
                }
            }
            return fiInfo.ToArray();
        }

        public static String[] ImagesAppendInfo(this HtmlHelper h, String Area, String Controller, Int32 Id, String FilesKind)
        {
            return ImagesAppendInfo(h, Area, Controller, Id, FilesKind, "OriginFile");
        }
        public static String[] ImagesAppendInfo(this HtmlHelper h, String Area, String Controller, Int32 Id, String FilesKind, int SizeParm)
        {
            return ImagesAppendInfo(h, Area, Controller, Id, FilesKind, "s_" + SizeParm);
        }
        public static String[] ImagesAppendInfo(this HtmlHelper h, String Area, String Controller, Int32 Id, String FilesKind, String SizeKind)
        {
            String S = String.Format(SystemUpImagePathTpl, Area, Controller, Id, FilesKind, SizeKind);
            String F = h.ViewContext.HttpContext.Server.MapPath(S);
            List<String> I = new List<String>();
            if (Directory.Exists(F))
            {
                String[] fs = Directory.GetFiles(F);

                foreach (String f in fs)
                {
                    FileInfo fi = new FileInfo(f);
                    String P = UrlHelper.GenerateContentUrl(S + "/" + fi.Name, h.ViewContext.HttpContext);
                    I.Add(P);
                }
            }
            return I.ToArray();
        }

        public static String ImgSrc(this HtmlHelper h, String AreaName, String ContorllerName, Int32 Id, String FilesKind, Int32 ImageSizeTRype, String NonePicName)
        {
            String ImgSizeString = "s_" + ImageSizeTRype;
            String SearchPath = String.Format(SystemUpFilePathTpl, AreaName, ContorllerName, Id, FilesKind, ImgSizeString);
            String FolderPth = h.ViewContext.HttpContext.Server.MapPath(SearchPath);

            if (Directory.Exists(FolderPth))
            {
                String[] SFiles = Directory.GetFiles(FolderPth);

                if (SFiles.Length > 0)
                {
                    FileInfo f = new FileInfo(SFiles[0]);
                    return UrlHelper.GenerateContentUrl(SearchPath, h.ViewContext.HttpContext) + "/" + f.Name;
                }
                else
                    return UrlHelper.GenerateContentUrl("~\\Content\\images\\" + NonePicName, h.ViewContext.HttpContext);

            }
            else
                return UrlHelper.GenerateContentUrl("~\\Content\\images\\" + NonePicName, h.ViewContext.HttpContext);
        }

        #endregion

        #region File Upload UI Helper

        public static MvcHtmlString FileFineUpLoad(this HtmlHelper helper,
            String divId, String title,
            String OpenDilaogElementId,
            String ajax_url_UpLoadFiles,
            String ajax_url_ListFiles,
            String ajax_url_DeleteFiles,
            int SystemId, String queryFileKind, Int32 height, Int32 Width)
        {
            AjaxFineUpLoaderUI ajaxUI = new AjaxFineUpLoaderUI(divId, "fine_uploader_" + divId, OpenDilaogElementId, ajax_url_UpLoadFiles, ajax_url_ListFiles, ajax_url_DeleteFiles);

            ajaxUI.Options.element = "$('#fine_uploader_" + divId + "')";
            ajaxUI.Options.request.inputName = "hd_FileUp_EL";
            ajaxUI.Options.request.endpoint = ajax_url_UpLoadFiles;
            ajaxUI.Options.request._params = new DataModule();
            ajaxUI.Options.request._params.Add("id", SystemId.ToString());
            ajaxUI.Options.request._params.Add("FilesKind", "'" + queryFileKind + "'");
            ajaxUI.Options.callbacks = new AjaxFineUpLoaderUI.callbackOptons();
            //$(this).fineUploader('clearStoredFiles');
            ajaxUI.Options.callbacks.complete = new funcMethodModule() { MakeStyle = funcMethodModule.funcMakeStyle.funcConext };
            ajaxUI.Options.callbacks.complete.funcString = String.Format("$.FilesListHandle('{0}_filesList','{1}','{2}',{3},'{4}');", divId, ajax_url_ListFiles, ajax_url_DeleteFiles, SystemId, queryFileKind);
            ajaxUI.Options.callbacks.complete.funcString += "if($(this).fineUploader('getInProgress')==0)";
            ajaxUI.Options.callbacks.complete.funcString += "{$(this).fineUploader('clearStoredFiles');}";

            ajaxUI.Options.callbacks.error = new funcMethodModule() { MakeStyle = funcMethodModule.funcMakeStyle.funcConext };
            ajaxUI.Options.callbacks.error.funcString = "alert('file uploader error:' + errorReason);";
            ajaxUI.Options.text = new AjaxFineUpLoaderUI.textOptions() { uploadButton = "<i class='icon-plus icon-white'></i>" + Resources.Res.Button_Choice_Files };
            ajaxUI.Options.autoUpload = false;
            ajaxUI.Options.debug = true;
            ajaxUI.Options.failedUploadTextDisplay = new AjaxFineUpLoaderUI.failedUploadTextDisplayOptions() { mode = "custom", enableTooltip = true, responseProperty = "error", maxChars = 100 };

            ajaxUI.SubmitButton = new jqElementEvent(new jqSelector() { IdName = divId + "_SubmitFileButton" }) ;
            ajaxUI.SubmitButton.events.Add(new jqElementEvent.jqEvents() { htmlElementEvent= HtmlObjectEvent.click,
                                                                           funcString = "$('#fine_uploader_" + divId + "').fineUploader('uploadStoredFiles');"
            });

            jqButton btn_upd_fmwk = new jqButton(new jqSelector() { IdName = ajaxUI.SubmitButton.Id });
            btn_upd_fmwk.options.icons.primary = FrameworkIcons.ui_icon_arrowreturnthick_1_n;

            ajaxUI.Dialog.options.height = height;
            ajaxUI.Dialog.options.width = Width;

            ajaxUI.Dialog.options.open = new funcMethodModule() { MakeStyle = funcMethodModule.funcMakeStyle.funcConext };
            ajaxUI.Dialog.options.open.funcString = String.Format("$.FilesListHandle('{0}_filesList','{1}','{2}',{3},'{4}');{5};", divId, ajax_url_ListFiles, ajax_url_DeleteFiles, SystemId, queryFileKind, btn_upd_fmwk.ToScriptString());
            ajaxUI.Dialog.options.zIndex = 2;

            StringBuilder sb_Html = new StringBuilder();
            sb_Html.AppendLine(String.Format("<div id=\"{0}\" title=\"{1}\">", divId, title));
            sb_Html.AppendLine("<div id=\"fine_uploader_" + divId + "\"></div>");
            sb_Html.AppendLine(String.Format("<button id=\"{0}\" type=\"button\">{1}</button>", ajaxUI.SubmitButton.Id, Resources.Res.Button_Start_FilesUp));
            sb_Html.AppendLine(String.Format("<fieldset><legend class=\"ui-state-active edit-subtitle-caption\" style=\"width:100%;font-size: 0.95em;padding:5px 5px 5px 5px;font-weight:normal\">{1}</legend><div id=\"{0}_filesList\"></div></fieldset>", divId, Resources.Res.Info_Files_List));
            sb_Html.AppendLine("</div>");

            return MvcHtmlString.Create(sb_Html.ToString() + ajaxUI.ToScriptString().ToJqueryDocumentReady().ToScriptTag());
        }

        public static MvcHtmlString ImageFineUpLoad(this HtmlHelper helper,
            String divId, String title,
            String OpenDilaogElementId,
            String ajax_url_UpLoadFiles,
            String ajax_url_ListFiles,
            String ajax_url_DeleteFiles,
            int SystemId, String queryFileKind, Int32 height, Int32 Width)
        {
            AjaxFineUpLoaderUI ajaxUI = new AjaxFineUpLoaderUI(divId, "fine_uploader_" + divId, OpenDilaogElementId, ajax_url_UpLoadFiles, ajax_url_ListFiles, ajax_url_DeleteFiles);

            ajaxUI.Options.element = "$('#fine_uploader_" + divId + "')";
            ajaxUI.Options.request.inputName = "hd_FileUp_EL";
            ajaxUI.Options.request.endpoint = ajax_url_UpLoadFiles;
            ajaxUI.Options.request._params = new DataModule();
            ajaxUI.Options.request._params.Add("id", SystemId.ToString());
            ajaxUI.Options.request._params.Add("FilesKind", "'" + queryFileKind + "'");
            ajaxUI.Options.callbacks = new AjaxFineUpLoaderUI.callbackOptons();
            //$(this).fineUploader('clearStoredFiles');
            ajaxUI.Options.callbacks.complete = new funcMethodModule() { MakeStyle = funcMethodModule.funcMakeStyle.funcConext };
            ajaxUI.Options.callbacks.complete.funcString = String.Format("$.FilesListHandle('{0}_filesList','{1}','{2}',{3},'{4}');", divId, ajax_url_ListFiles, ajax_url_DeleteFiles, SystemId, queryFileKind);
            ajaxUI.Options.callbacks.complete.funcString += "if($(this).fineUploader('getInProgress')==0)";
            ajaxUI.Options.callbacks.complete.funcString += "{$(this).fineUploader('clearStoredFiles');}";

            ajaxUI.Options.callbacks.error = new funcMethodModule() { MakeStyle = funcMethodModule.funcMakeStyle.funcConext };
            ajaxUI.Options.callbacks.error.funcString = "alert('file uploader error:' + errorReason);";
            ajaxUI.Options.text = new AjaxFineUpLoaderUI.textOptions() { uploadButton = "<i class='icon-plus icon-white'></i>" + Resources.Res.Button_Choice_Images };
            ajaxUI.Options.autoUpload = false;
            ajaxUI.Options.debug = true;
            ajaxUI.Options.failedUploadTextDisplay = new AjaxFineUpLoaderUI.failedUploadTextDisplayOptions() { mode = "custom", enableTooltip = true, responseProperty = "error", maxChars = 100 };

            ajaxUI.Options.validation = new AjaxFineUpLoaderUI.validationOptions();
            ajaxUI.Options.validation.allowedExtensions = new String[] { "jpg", "jpeg", "gif", "png" };

            ajaxUI.SubmitButton = new jqElementEvent(new jqSelector() { IdName = divId + "_SubmitFileButton" });
            ajaxUI.SubmitButton.events.Add(new jqElementEvent.jqEvents() { htmlElementEvent= HtmlObjectEvent.click
            , funcString = "$('#fine_uploader_" + divId + "').fineUploader('uploadStoredFiles');"
            });

            jqButton btn_upd_fmwk = new jqButton(new jqSelector() { IdName = ajaxUI.SubmitButton.Id });
            btn_upd_fmwk.options.icons.primary = FrameworkIcons.ui_icon_arrowreturnthick_1_n;

            ajaxUI.Dialog.options.open = new funcMethodModule() { MakeStyle = funcMethodModule.funcMakeStyle.funcConext };
            ajaxUI.Dialog.options.open.funcString = String.Format("$.FilesListHandle('{0}_filesList','{1}','{2}',{3},'{4}');{5};", divId, ajax_url_ListFiles, ajax_url_DeleteFiles, SystemId, queryFileKind, btn_upd_fmwk.ToScriptString());
            ajaxUI.Dialog.options.zIndex = 2;

            ajaxUI.Dialog.options.height = height;
            ajaxUI.Dialog.options.width = Width;

            StringBuilder sb_Html = new StringBuilder();
            sb_Html.AppendLine(String.Format("<div id=\"{0}\" title=\"{1}\">", divId, title));
            sb_Html.AppendLine("<div id=\"fine_uploader_" + divId + "\"></div>"); //此div交由FineUpload控制
            sb_Html.AppendLine(String.Format("<button id=\"{0}\" type=\"button\">{1}</button>", ajaxUI.SubmitButton.Id, Resources.Res.Button_Start_ImagesUp));
            sb_Html.AppendLine(String.Format("<fieldset><legend class=\"ui-state-active edit-subtitle-caption\" style=\"width:100%;font-size: 0.95em;padding:5px 5px 5px 5px;font-weight:normal\">{1}</legend><div id=\"{0}_filesList\"></div></fieldset>", divId, Resources.Res.Info_Images_List));
            sb_Html.AppendLine("</div>");

            return MvcHtmlString.Create(sb_Html.ToString() + ajaxUI.ToScriptString().ToJqueryDocumentReady().ToScriptTag());
        }

        #endregion

        public static String PFile(this HtmlHelper h, String f)
        {
            var pagePath = ((WebViewPage)(h.ViewDataContainer)).VirtualPath;
            var filePath = h.ViewContext.HttpContext.Server.MapPath( pagePath.Substring(0, pagePath.LastIndexOf('/') + 1) + "//" + f);

            String r = String.Empty;
            if (System.IO.File.Exists(filePath))
            {
                StreamReader sr = new StreamReader(filePath);
                r = sr.ReadToEnd();
                sr.Close();
            }

            return r;
        }
    }

    public static class SystemFilesExportHelper
    {
        public static String ExportImgSystemFile(this HtmlHelper htmlHelper, String WebPath, String AreaName, String ContorllerName, String Id, String GroupType)
        {
            String tpl = htmlHelper.ViewContext.HttpContext.Request.PhysicalApplicationPath + @"_Code\SysUpFiles\" + AreaName + "." + ContorllerName + @"\" + Id + @"\" + GroupType + "\\OriginSize";

            if (Directory.Exists(tpl))
            {
                String[] SFiles = Directory.GetFiles(tpl);

                if (SFiles.Length > 0)
                {
                    FileInfo f = new FileInfo(SFiles[0]);
                    return WebPath + @"_Code\SysUpFiles\" + AreaName + "." + ContorllerName + @"\" + Id + @"\" + GroupType + "\\OriginSize\\" + f.Name;
                }
                else
                {
                    return WebPath + @"_Web\Images\NoPic\None.jpg";
                }
            }
            else
            {
                return WebPath + @"_Web\Images\NoPic\None.jpg";
            }
        }
    }
    public static class LocalizationHelpe
    {
        public static String Lang(this HtmlHelper htmlHelper, String key)
        {
            return Lang(htmlHelper.ViewDataContainer as WebViewPage, key);
        }

        public static String Lang<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        where TModel : class
        {
            var inputName = ExpressionHelper.GetExpressionText(expression);
            return Lang(htmlHelper.ViewDataContainer as WebViewPage, inputName);
        }

        public static String Lang<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, String prefx)
        where TModel : class
        {
            var inputName = prefx + ExpressionHelper.GetExpressionText(expression);
            return Lang(htmlHelper.ViewDataContainer as WebViewPage, inputName);
        }

        /// <summary>
        /// Grid欄位專用，為必免命名衝突以g_為開頭在加欄位名稱
        /// </summary>
        public static String GLang<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        where TModel : class
        {
            return Lang<TModel, TProperty>(htmlHelper, expression, "g_");
        }
        public static String GLang(this HtmlHelper htmlHelper, String key)
        {
            String s = "g_" + key;
            return Lang(htmlHelper.ViewDataContainer as WebViewPage, s);
        }
        public static String GLang(this HtmlHelper htmlHelper, FieldModule fm)
        {
            String s = "g_" + fm.M;
            return Lang(htmlHelper.ViewDataContainer as WebViewPage, s);
        }


        /// <summary>
        /// 編輯表欄位專用以，為必免命名衝突以f_為開頭在加欄位名稱
        /// </summary>
        public static String FieldLang<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        where TModel : class
        {
            return Lang<TModel, TProperty>(htmlHelper, expression, "f_");
        }
        public static String FLang(this HtmlHelper htmlHelper, String key)
        {
            String s = "f_" + key;
            return Lang(htmlHelper.ViewDataContainer as WebViewPage, s);
        }
        public static String FLang(this HtmlHelper htmlHelper, FieldModule fm)
        {
            String s = "f_" + fm.M;
            return Lang(htmlHelper.ViewDataContainer as WebViewPage, s);
        }

        /// <summary>
        /// 編輯表欄提示訊息專用，為必免命名衝突以t_為開頭在加欄位名稱
        /// </summary>
        public static String TipLang<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        where TModel : class
        {
            return Lang<TModel, TProperty>(htmlHelper, expression, "t_");
        }
        public static String TLang(this HtmlHelper htmlHelper, String key)
        {
            string s = "t_" + key;
            return Lang(htmlHelper.ViewDataContainer as WebViewPage, s);
        }


        private static IEnumerable<DictionaryEntry> GetResx(String LocalResourcePath)
        {
            ObjectCache cache = MemoryCache.Default;
            IEnumerable<DictionaryEntry> resxs = null;

            if (cache.Contains(LocalResourcePath))
                resxs = cache.GetCacheItem(LocalResourcePath).Value as IEnumerable<DictionaryEntry>;
            else
            {
                if (File.Exists(LocalResourcePath))
                {
                    resxs = new ResXResourceReader(LocalResourcePath).Cast<DictionaryEntry>();
                    cache.Add(LocalResourcePath, resxs, new CacheItemPolicy() { Priority = CacheItemPriority.NotRemovable });
                }
            }

            return resxs;
        }

        public static String Lang(this WebPageBase page, String key)
        {
            var pagePath = page.VirtualPath;
            var pageName = pagePath.Substring(pagePath.LastIndexOf('/'), pagePath.Length - pagePath.LastIndexOf('/')).TrimStart('/');
            var filePath = page.Server.MapPath(pagePath.Substring(0, pagePath.LastIndexOf('/') + 1)) + "App_LocalResources";

            String lang = System.Globalization.CultureInfo.CurrentCulture.Name;
            String resxKey = String.Empty;

            resxKey =
                lang == "zh-TW" || String.IsNullOrWhiteSpace(lang) ?
                String.Format(@"{0}\{1}.resx", filePath, pageName) :
                String.Format(@"{0}\{1}.{2}.resx", filePath, pageName, lang);

            IEnumerable<DictionaryEntry> resxs = GetResx(resxKey);

            if (resxs != null)
                return (String)resxs.FirstOrDefault<DictionaryEntry>(x => x.Key.ToString() == key).Value;
            else
                return "";
        }
    }

    #region Form Help
    public class MVCFormReport : IDisposable
    {
        private Boolean disposed;
        private ViewContext _vc;

        public MVCFormReport(ViewContext VCText, String AjaxFormId, String ActionUrl, Boolean UseReturn, String SubmitId)
        {
            HandleInit(VCText, AjaxFormId, ActionUrl, SubmitId);
        }
        public MVCFormReport(ViewContext VCText, String AjaxFormId, String ActionUrl)
        {
            HandleInit(VCText, AjaxFormId, ActionUrl, "btn_submit");
        }
        public MVCFormReport(ViewContext VCText, String AjaxFormId)
        {
            HandleInit(VCText, AjaxFormId, "ajax_MasterUpdata", "btn_submit");
        }

        private void HandleInit(ViewContext VCText, String AjaxFormId, String ActionUrl, String SubmitId)
        {
            _vc = VCText;
            FormId = AjaxFormId;
            action = ActionUrl;

            //                "/" + _vc.RouteData.DataTokens["area"].ToString() + "/" + _vc.RouteData.Values["controller"].ToString() + "/" +

            String tpl = String.Format("<form id=\"{0}\" name=\"{0}\" method=\"{1}\" action=\"{2}\" enctype=\"{3}\">\r\n", FormId, method, action, GetEnctype());
            _vc.Writer.Write(tpl);

//            SubmitElement = new jqElementEvent(new jqSelector() { IdName = SubmitId }) { Event = HtmlObjectEvent.click };
//            SubmitElement.funcObject.funcString = @"
//                ajaxHasDone=$.when.apply( $, ajaxRequest ); 
//                ajaxHasDone.done(function() {  
//                    if($('#" + FormId + @"').valid()) 
//                            $('#" + FormId + @"').submit();
//                });";

            ajaxOptions = new FormAjaxOption()
            {
                beforeSubmit = new funcMethodModule() { funcName = "AjaxFormBeforeSubmit", MakeStyle = funcMethodModule.funcMakeStyle.onlyName },
                success = new funcMethodModule() { funcName = "AjaxFormSuccess", MakeStyle = funcMethodModule.funcMakeStyle.onlyName },
                dataType = "json",
                contentType = "application/x-www-form-urlencoded; charset=utf-8"
            };

            ajaxOptions.beforeSubmit.funcString = "var queryString = $.param(formData);return true;";
            ajaxOptions.success.funcString = "if (jsonobj.result)";
            ajaxOptions.success.funcString += "{ $('#ifm_exceldownload').attr('src','ExportExcelFile?FileName=' + jsonobj.filesObject[0].FileName + '&WebPath=' + jsonobj.filesObject[0].OriginFilePath);}";
            ajaxOptions.success.funcString += "if(jsonobj.message!='') $.UiMessage(jsonobj);";

            formValidate = new FormValidateSetup(new jqSelector() { IdName = this.FormId });
        }

        public String FormId { get; set; }
        public formMethods method { get; set; }
        public String action { get; set; }
        public formEnctype enctype { get; set; }
        public String GetEnctype()
        {

            if (enctype == formEnctype.application_x_www_form_urlencoded)
                return "application/x-www-form-urlencoded";

            else if (enctype == formEnctype.multipart_form_data)
                return "multipart/form-data";

            else
                return "text/plain";
        }

        public jqElementEvent SubmitElement { get; set; }
        public FormAjaxOption ajaxOptions { get; set; }
        public FormValidateSetup formValidate { get; set; }
        public String LastAppenScript { get; set; }
        private void ToScriptSting()
        {
            _vc.Writer.Write("</form>\r\n");

            String script = String.Empty;

            if (SubmitElement != null)
                script += SubmitElement.ToScriptString();

            if (ajaxOptions != null)
            {
                String tpl = "\tvar options={0};\t\r\n{1};\t\r\n{2};\t\r\n{3};";
                MakeJqScript createJsonStrig = new MakeJqScript() { GetObject = this.ajaxOptions };
                //createJsonStrig.AddPropertyOption(new PropertyObjectJsonOption() { PropertyName = "ajaxOptions" });
                script += String.Format(tpl,
                    createJsonStrig.MakeScript(),
                    ajaxOptions.beforeSubmit.ToScriptString(funcMethodModule.funcMakeStyle.complete),
                    ajaxOptions.success.ToScriptString(funcMethodModule.funcMakeStyle.complete),
                    formValidate.ToScriptString()
                    );
                tpl = null; createJsonStrig = null;
            }

            //jqElementEvent jqFormSubmit = new jqElementEvent(new jqSelector() { IdName = this.FormId }) { Event = HtmlObjectEvent.submit };
            //jqFormSubmit.funcObject.funcString = "$(this).ajaxSubmit(options);return false;";
            //script += jqFormSubmit.ToScriptString();
            //jqFormSubmit = null;
            script += LastAppenScript;

            _vc.Writer.Write(script.ToJqueryDocumentReady().ToScriptTag());
            script = null;
        }

        public void FormEnd()
        {
            ToScriptSting();
            Dispose();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {

                }
                disposed = true;
            }
        }
    }
    public class TagInstanct : IDisposable
    {
        private ViewContext _vwContext;
        private String _tagName;

        public TagInstanct(ViewContext VCText, String TagName)
            : base()
        {
            this._vwContext = VCText;
            this._tagName = TagName;
            _vwContext.Writer.Write("<" + TagName + ">");
        }

        public TagInstanct(ViewContext VCText, String TagName, String className)
            : base()
        {
            this._vwContext = VCText;
            this._tagName = TagName;
            TagBuilder t = new TagBuilder(TagName);
            if (className != null)
                t.AddCssClass(className);
            _vwContext.Writer.Write(t.ToString((TagRenderMode.StartTag)));
        }

        public void Dispose()
        {
            _vwContext.Writer.Write("</" + this._tagName + ">");
            _vwContext = null;
            GC.SuppressFinalize(this);
        }

    }
    #endregion
    public class MakeColumnModule
    {
        public MakeColumnModule()
        {
            CM = new jqGrid.colObject();
        }
        /// <summary>
        /// Grid欄位顯示的名稱
        /// </summary>
        public String CN { get; set; }
        /// <summary>
        /// Grid的Jquery ColumnModel
        /// </summary>
        public jqGrid.colObject CM { get; set; }
    }
}