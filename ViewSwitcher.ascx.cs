// CLOUD READINESS MIGRATION (cr-dotnet-0026):
// Migrated from ASP.NET Web Forms UserControl to ASP.NET Core MVC/Razor Pages pattern.
// System.Web.UI.UserControl replaced with ASP.NET Core ViewComponent or Partial View.
// Microsoft.AspNet.FriendlyUrls.Resolvers (IIS-specific) replaced with ASP.NET Core
// middleware-based mobile detection for cloud-native deployment on AWS (ECS/EKS).
// WebFormsFriendlyUrlResolver.IsMobileView() replaced with IHttpContextAccessor
// and user-agent detection compatible with Kestrel web server.
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FIlms
{
    /// <summary>
    /// ViewSwitcher component - migrated from ASP.NET Web Forms UserControl to ASP.NET Core ViewComponent.
    /// Replaces System.Web.UI.UserControl and Microsoft.AspNet.FriendlyUrls.Resolvers with
    /// ASP.NET Core IHttpContextAccessor for cloud-native mobile detection on AWS.
    /// IIS-specific WebFormsFriendlyUrlResolver replaced with cross-platform user-agent detection.
    /// </summary>
    public class ViewSwitcherViewComponent : ViewComponent
    {
        private readonly ILogger<ViewSwitcherViewComponent> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Properties replacing Web Forms code-behind protected properties
        public string CurrentView { get; private set; }
        public string AlternateView { get; private set; }
        public string SwitchUrl { get; private set; }

        public ViewSwitcherViewComponent(
            ILogger<ViewSwitcherViewComponent> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public IViewComponentResult Invoke()
        {
            var context = _httpContextAccessor.HttpContext;

            // Replaces WebFormsFriendlyUrlResolver.IsMobileView() - IIS-specific dependency
            // Uses cross-platform user-agent detection compatible with Kestrel on AWS
            var userAgent = context?.Request.Headers["User-Agent"].ToString() ?? string.Empty;
            var isMobile = IsMobileUserAgent(userAgent);

            CurrentView = isMobile ? "Mobile" : "Desktop";
            AlternateView = isMobile ? "Desktop" : "Mobile";

            // Build switch URL using ASP.NET Core routing instead of IIS FriendlyUrls
            var returnUrl = context?.Request.Path.Value ?? "/";
            SwitchUrl = $"/ViewSwitcher/SwitchView?view={AlternateView}&ReturnUrl={Uri.EscapeDataString(returnUrl)}";

            return View(this);
        }

        /// <summary>
        /// Cross-platform mobile user-agent detection replacing IIS-specific
        /// WebFormsFriendlyUrlResolver.IsMobileView() for AWS/Kestrel compatibility.
        /// </summary>
        private static bool IsMobileUserAgent(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
                return false;

            return userAgent.Contains("Mobile", StringComparison.OrdinalIgnoreCase)
                || userAgent.Contains("Android", StringComparison.OrdinalIgnoreCase)
                || userAgent.Contains("iPhone", StringComparison.OrdinalIgnoreCase)
                || userAgent.Contains("iPad", StringComparison.OrdinalIgnoreCase)
                || userAgent.Contains("Windows Phone", StringComparison.OrdinalIgnoreCase);
        }
    }
}
