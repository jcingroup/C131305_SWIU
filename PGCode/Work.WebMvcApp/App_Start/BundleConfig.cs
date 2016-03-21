using System.Web;
using System.Web.Optimization;

namespace DotWeb.AppStart
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/_Code/jqScript/Comm.jqScript")
            .Include(
            "~/_Code/jqScript/jquery.unobtrusive-ajax.js",
            "~/_Code/jqScript/jquery.query-2.1.7.js",
            "~/_Code/jqScript/commfunc.js"));

            bundles.Add(new ScriptBundle("~/_Code/jqScript/Comm.jqScript.jqGrid")
            .Include(
            "~/_Code/jqScript/ui.jquery.jqGrid-4.5.2/js/i18n/grid.locale-tw.js",
            "~/_Code/jqScript/ui.jquery.jqGrid-4.5.2/js/jquery.jqGrid.src.js"));

            bundles.Add(new ScriptBundle("~/_Code/jqScript/Comm.jqScript.Fileuploader")
            .Include(
            "~/_Code/jqScript/Fileuploader/jquery.fineuploader-3.0.js"));

            bundles.Add(new ScriptBundle("~/_Code/jqScript/Comm.jqScript.fancybox")
            .Include(
            "~/_Code/jqScript/fancyBox/source/jquery.fancybox.js"));


            bundles.Add(new StyleBundle("~/_Code/CSS/Comm.CSS").Include(
            "~/_Code/CSS/formStyle.css",
            "~/_Code/CSS/commStyle.css"));

            bundles.Add(new StyleBundle("~/_Code/CSS/Comm.CSS.jqGrid").Include(
            "~/_Code/jqScript/ui.jquery.jqGrid-4.5.2/css/ui.jqgrid.css"));

            bundles.Add(new StyleBundle("~/_Code/CSS/Comm.CSS.fineuploader").Include(
            "~/_Code/jqScript/Fileuploader/fineuploader.css"));

            bundles.Add(new StyleBundle("~/_Code/CSS/Comm.CSS.fancybox").Include(
            "~/_Code/jqScript/fancyBox/source/jquery.fancybox.css"));

            BundleTable.EnableOptimizations = false;
        }
    }
}