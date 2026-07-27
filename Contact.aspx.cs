// CLOUD READINESS MIGRATION (cr-dotnet-0026):
// Migrated from ASP.NET Web Forms to ASP.NET Core MVC/Razor Pages pattern.
// System.Web.UI.Page replaced with Microsoft.AspNetCore.Mvc.RazorPages.PageModel.
// This code-behind is refactored to use ASP.NET Core Razor Pages conventions
// for cloud-native deployment on AWS (ECS/EKS) using Kestrel web server.
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace FIlms
{
    /// <summary>
    /// Contact page model - migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
    /// Replaces System.Web.UI.Page inheritance with PageModel for cloud-native deployment.
    /// </summary>
    public class ContactModel : PageModel
    {
        private readonly ILogger<ContactModel> _logger;

        public ContactModel(ILogger<ContactModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            // Page load logic - stateless GET handler replacing Page_Load event
        }
    }
}
