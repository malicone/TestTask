using System.Web;
using System.Web.Optimization;

namespace TestTask
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles( BundleCollection bundles )
        {
            bundles.Add( new ScriptBundle( "~/bundles/jquery" ).Include(
                        "~/Scripts/jquery-{version}.js" ) );

            bundles.Add( new ScriptBundle( "~/bundles/jqueryval" ).Include(
                        "~/Scripts/jquery.validate*" ) );

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add( new ScriptBundle( "~/bundles/modernizr" ).Include(
                        "~/Scripts/modernizr-*" ) );

            bundles.Add( new ScriptBundle( "~/bundles/bootstrap" ).Include(
                      // max 20190531: change from bootstrap.js to bootstrap.bundle.js fixed dropdown not shown at all and
                      // items shown in one line.
                      // https://stackoverflow.com/questions/46026899/bootstrap-4-dropdown-menu-not-working
                      "~/Scripts/bootstrap.bundle.js",
                      "~/Scripts/popper.min.js" ) );

            bundles.Add( new StyleBundle( "~/Content/css" ).Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css" ) );

        }
    }
}
