// CLOUD READINESS MIGRATION (cr-dotnet-0026):
// Migrated from ASP.NET Web Forms MasterPage to ASP.NET Core MVC/Razor Pages pattern.
// System.Web.UI.MasterPage replaced with Microsoft.AspNetCore.Mvc.RazorPages.PageModel.
// This code-behind is refactored to use ASP.NET Core Razor Pages conventions
// for cloud-native deployment on AWS (ECS/EKS) using Kestrel web server.
// The mobile master page is replaced with a responsive ASP.NET Core layout.
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace FIlms
{
    /// <summary>
    /// Mobile site layout model - migrated from ASP.NET Web Forms MasterPage to ASP.NET Core Razor Pages.
    /// Replaces System.Web.UI.MasterPage inheritance with PageModel for cloud-native deployment.
    /// Mobile detection is handled via ASP.NET Core middleware and responsive CSS instead of
    /// separate mobile master pages, enabling horizontal scalability on AWS.
    /// </summary>
    public class SiteMobileModel : PageModel
    {
        private readonly ILogger<SiteMobileModel> _logger;

        public SiteMobileModel(ILogger<SiteMobileModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            // Stateless GET handler replacing Page_Load event
            // Mobile view switching is handled by ASP.NET Core middleware
        }
    }
}
