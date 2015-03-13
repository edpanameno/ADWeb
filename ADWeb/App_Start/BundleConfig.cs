using System.Web;
using System.Web.Optimization;

namespace ADWeb
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Content/js/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Content/js/jquery.validate*",
                        "~/Content/js/jquery.unobtrusive-ajax*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Content/js/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Content/js/bootstrap.js",
                      "~/Content/js/respond.js"));

            bundles.Add(new StyleBundle("~/content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/custom.css",
                      "~/Content/themes/base/core.css",
                      "~/Content/themes/base/autocomplete.css"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                "~/Content/js/jquery-ui-1.11.3.min.js"));

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = true;
        }
    }
}
