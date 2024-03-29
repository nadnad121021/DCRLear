﻿using System.Web;
using System.Web.Optimization;
using WebHelpers.Mvc5;

namespace DCRSystem
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.validate*"));

            //// Use the development version of Modernizr to develop with and learn from. Then, when you're
            //// ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.js"));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          "~/Content/bootstrap.css",
            //          "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Bundles/css")
                .Include("~/Content/css/bootstrap.min.css", new CssRewriteUrlTransformAbsolute())
                .Include("~/Content/css/bootstrap-select.css")
                .Include("~/Content/css/bootstrap-datepicker3.min.css")
                .Include("~/Content/css/font-awesome.min.css", new CssRewriteUrlTransformAbsolute())
                .Include("~/Content/css/icheck/blue.min.css", new CssRewriteUrlTransformAbsolute())
                .Include("~/Content/css/AdminLTE.css", new CssRewriteUrlTransformAbsolute())
                .Include("~/Content/css/skins/_all-skins.css")
                .Include("~/Content/CDN/Datatable/css/jquery.dataTables.min.css")
                .Include("~/Content/plugins/pace/css/pace.css")
                .Include("~/Content/plugins/pace/css/pace.min.css")
                .Include("~/Content/select2/css/select2.css")
                .Include("~/Content/select2/css/select2.min.css")
                .Include("~/Content/css/skins/custom-red.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                     "~/Content/css/font-awesome.min.css",
                     "~/Content/css/main.css"));

            bundles.Add(new StyleBundle("~/Bundles/login-css")
                .Include("~/Content/css/font-awesome.min.css")
                .Include("~/Content/css/login-main.css"));


            bundles.Add(new ScriptBundle("~/Bundles/login-js")
                .Include("~/Scripts/jquery-{version}.js")
                .Include("~/Scripts/popper.min.js")
                .Include("~/Scripts/bootstrap.min.js")
                .Include("~/Scripts/main.js")
                .Include("~/Scripts/pace.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                       "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/Bundles/js")
                .Include("~/Content/js/plugins/jquery/jquery-3.3.1.js")
                .Include("~/Content/js/plugins/bootstrap/bootstrap.js")
                .Include("~/Content/js/plugins/fastclick/fastclick.js")
                .Include("~/Content/js/plugins/slimscroll/jquery.slimscroll.js")
                .Include("~/Content/js/plugins/bootstrap-select/bootstrap-select.js")
                .Include("~/Content/js/plugins/moment/moment.js")
                .Include("~/Content/js/plugins/datepicker/bootstrap-datepicker.js")
                .Include("~/Content/js/plugins/icheck/icheck.js")
                .Include("~/Content/js/plugins/validator/validator.js")
                .Include("~/Content/js/plugins/inputmask/jquery.inputmask.bundle.js")
                .Include("~/Content/js/adminlte.js")
                .Include("~/Content/js/init.js")
                .Include("~/Content/CDN/Datatable/js/jquery.dataTables.min.js")
                .Include("~/Content/plugins/pace/js/pace.js")
                .Include("~/Content/plugins/pace/js/pace.min.js")
                .Include("~/Scripts/pace/pace.js")
                .Include("~/Scripts/Widgets/Widgets-menu.js")
                .Include("~/Content/select2/js")
                .Include("~/Scripts/Forms/Advanced"));
           

#if DEBUG
            BundleTable.EnableOptimizations = false;
#else
            BundleTable.EnableOptimizations = true;
#endif
        }
    }
}
