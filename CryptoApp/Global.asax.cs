using CryptoApp.App_Start;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace CryptoApp
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //RegisterDlls("SQLite.Interop");
            InitSqlite.Initialize();
            System.Web.HttpContext.Current.SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior.Required);
        }
        protected void Application_PostAuthorizeRequest()
        {
            System.Web.HttpContext.Current.SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior.Required);
        }
        protected void Application_BeginRequest()
        {
            if (HttpContext.Current.Session != null)
            {
                HttpContext.Current.Session["refresh"] = DateTime.Now.Ticks;
            }
        }


        //This should be called with the names of the dlls
        private void RegisterDlls(params string[] dllNames)
        {
            AppDomain curDomain = AppDomain.CurrentDomain;
            //The directory must be present in two kinds in two subdirs of the website
            //at the same level of the "bin" directory:
            //binnative/x86 and binnative/x64
            
            String shadowCopyDir = curDomain.DynamicDirectory;

            foreach (var dllName in dllNames)
            {
                String binDir32 = Path.Combine(curDomain.BaseDirectory, "bin",  "x86");
                String binDir64 = Path.Combine(curDomain.BaseDirectory, "bin","x64");
                if(!NewMethod(curDomain, binDir32, shadowCopyDir, dllName))
                {
                    NewMethod(curDomain, binDir64, shadowCopyDir, dllName);
                }
            }
        }

        private static bool NewMethod(AppDomain curDomain, string binDir, string shadowCopyDir, string dllName)
        {
            String dllSrc = Path.Combine(binDir, dllName + ".dll");
            String dllDst = Path.Combine(shadowCopyDir, Path.GetFileName(dllSrc));

            try
            {
                //The files are copied on the shadow copy areas
                File.Copy(dllSrc, dllDst, true);
                //And loaded explicitely!
                Assembly.LoadFrom(dllDst);
            }
            catch (System.Exception ex)
            {
                var res = Path.Combine(curDomain.BaseDirectory, "log.txt");
                File.AppendAllText(res, binDir+"/"+dllName+"\n"+ ex.ToString() + "\n" + ex.StackTrace);
                return false;
            }
            return true;
        }
    }
}
