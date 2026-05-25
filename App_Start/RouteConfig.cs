using System;
using System.Web.Routing;
using Microsoft.AspNet.FriendlyUrls;

namespace FIlms
{
    /// <summary>
    /// Configuration for URL routing
    /// </summary>
    public static class RouteConfig
    {
        /// <summary>
        /// Registers routes for the application
        /// </summary>
        /// <param name="routes">Route collection</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            // Enable Friendly URLs for WebForms
            var settings = new FriendlyUrlSettings();
            settings.AutoRedirectMode = RedirectMode.Permanent;
            routes.EnableFriendlyUrls(settings);
        }
    }
}
