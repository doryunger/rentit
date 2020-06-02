using CarRent.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace RentIt
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Database.SetInitializer<CarRentContext>(null);            
            string root = Server.MapPath("~");
            string parent = Path.GetDirectoryName(root);
            string grandParent = Path.GetDirectoryName(parent);
            string newPathDataDiretory = grandParent + "\\CarRent.DAL";
            AppDomain.CurrentDomain.SetData("DataDirectory", newPathDataDiretory);
            var b = AppDomain.CurrentDomain.GetData("DataDirectory");
        }
        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            HttpCookie authCookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie == null || authCookie.Value == "")
                return;

            FormsAuthenticationTicket authTicket;
            try
            {
                authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            }
            catch
            {
                return;
            }

            // retrieve roles from UserData
            string[] roles = authTicket.UserData.Split(';');

            if (Context.User != null)
                Context.User = new GenericPrincipal(Context.User.Identity, roles);
        }
    }
}
