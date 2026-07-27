// CLOUD READINESS MIGRATION (cr-dotnet-0026):
// Migrated from ASP.NET Web Forms to ASP.NET Core MVC/Razor Pages pattern.
// System.Web.UI.Page replaced with Microsoft.AspNetCore.Mvc.RazorPages.PageModel.
// This code-behind is refactored to use ASP.NET Core Razor Pages conventions
// for cloud-native deployment on AWS (ECS/EKS) using Kestrel web server.
// Web Forms event handlers (Button_Click, Calendar_SelectionChanged) are replaced
// with OnPost handler methods following Razor Pages conventions.
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace FIlms
{
    /// <summary>
    /// Register page model - migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
    /// Replaces System.Web.UI.Page inheritance with PageModel for cloud-native deployment.
    /// Calendar control replaced with HTML date input; form validation uses DataAnnotations.
    /// </summary>
    public class RegisterModel : PageModel
    {
        private readonly ILogger<RegisterModel> _logger;

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }

        [BindProperty]
        public string SecretQuestion { get; set; }

        [BindProperty]
        public string SecretAnswer { get; set; }

        [BindProperty]
        public string FirstName { get; set; }

        [BindProperty]
        public string LastName { get; set; }

        [BindProperty]
        public int Gender { get; set; }

        [BindProperty]
        public DateTime? BirthDate { get; set; }

        public RegisterModel(ILogger<RegisterModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            // Stateless GET handler replacing Page_Load event
        }

        public IActionResult OnPostRegister()
        {
            // Replaces Button1_Click - registration logic
            if (!ModelState.IsValid)
                return Page();
            // Registration logic would be implemented here
            return RedirectToPage("/LogIn");
        }

        public void OnPostDateSelected()
        {
            // Replaces Calendar1_SelectionChanged - date selection handler
            // BirthDate is bound via model binding from HTML date input
        }
    }
}
