using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using FIlms;

namespace FIlms
{
    /// <summary>
    /// Cloud-ready Global Application Class
    /// Note: For full cloud compatibility, migrate to ASP.NET Core with Kestrel
    /// This removes IIS dependencies and enables containerization on Azure Container Apps
    /// </summary>
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterOpenAuth();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            
            // Cloud-ready: Add structured logging for cloud monitoring
            // Consider migrating to Application Insights for Azure
            // Example: TelemetryConfiguration.Active.InstrumentationKey = ConfigurationManager.AppSettings["ApplicationInsightsKey"];
        }

        void Application_End(object sender, EventArgs e)
        {
            // Code that runs on application shutdown
            // Cloud-ready: Ensure graceful shutdown for containerized environments
        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs
            // Cloud-ready: Log errors to centralized logging service (e.g., Application Insights)
            Exception ex = Server.GetLastError();
            if (ex != null)
            {
                // TODO: Implement cloud-native error logging
                // Example: TelemetryClient.TrackException(ex);
            }
        }
    }
}
