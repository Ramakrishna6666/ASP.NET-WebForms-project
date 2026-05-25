using System.Web.Optimization;

namespace FIlms
{
    /// <summary>
    /// Configuration for bundling and minification of CSS and JavaScript files
    /// </summary>
    public class BundleConfig
    {
        /// <summary>
        /// Registers bundles for the application
        /// </summary>
        /// <param name="bundles">Bundle collection</param>
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Script bundles
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/WebFormsJs").Include(
                        "~/Scripts/WebForms/WebForms.js",
                        "~/Scripts/WebForms/WebUIValidation.js",
                        "~/Scripts/WebForms/MenuStandards.js",
                        "~/Scripts/WebForms/Focus.js",
                        "~/Scripts/WebForms/GridView.js",
                        "~/Scripts/WebForms/DetailsView.js",
                        "~/Scripts/WebForms/TreeView.js",
                        "~/Scripts/WebForms/WebParts.js"));

            bundles.Add(new ScriptBundle("~/bundles/MsAjaxJs").Include(
                        "~/Scripts/WebForms/MsAjax/MicrosoftAjax.js",
                        "~/Scripts/WebForms/MsAjax/MicrosoftAjaxApplicationServices.js",
                        "~/Scripts/WebForms/MsAjax/MicrosoftAjaxTimer.js",
                        "~/Scripts/WebForms/MsAjax/MicrosoftAjaxWebForms.js"));

            // Style bundles
            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/Site.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));

            // Enable optimizations for production
            // BundleTable.EnableOptimizations = true;
        }
    }
}
