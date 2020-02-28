using System.Web;
using System.Web.Optimization;

namespace TaoZhugong
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/sb-admin2/Bootstrap_core_JavaScript").Include(
                "~/Content/sbadmin2/vendor/jquery/jquery.min.js",
                "~/Content/sbadmin2/vendor/bootstrap/js/bootstrap.bundle.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/sb-admin2/Core_plugin_JavaScript").Include(
                "~/Content/sbadmin2/vendor/jquery-easing/jquery.easing.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/sb-admin2/Custom_scripts_for_all_pages").Include(
                "~/Content/sbadmin2/js/sb-admin-2.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/sb-admin2/Page_level_plugins").Include(
                "~/Content/sbadmin2/vendor/chart.js/Chart.min.js",
                "~/Content/sbadmin2/vendor/datatables/jquery.dataTables.min.js",
                "~/Content/sbadmin2/vendor/datatables/dataTables.bootstrap4.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/sb-admin2/Page_level_custom_scripts").Include(
                "~/Content/sbadmin2/js/demo/datatables-demo.js",
                "~/Content/sbadmin2/js/demo/chart-area-demo.js",
                "~/Content/sbadmin2/js/demo/chart-pie-demo.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
            bundles.Add(new StyleBundle("~/Content/sb-admin-2css").Include(
                "~/Content/sbadmin2/vendor/fontawesome-free/css/all.min.css",
                "~/Content/sbadmin2/css/fontsStyle.css",
                "~/Content/sbadmin2/css/sb-admin-2.min.css",
                "~/Content/sbadmin2/vendor/datatables/dataTables.bootstrap4.min.css"
            ));
        }
    }
}
