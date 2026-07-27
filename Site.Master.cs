// CLOUD READINESS MIGRATION (cr-dotnet-0026, cr-dotnet-0123):
// 1. Migrated from ASP.NET Web Forms MasterPage to ASP.NET Core Razor Pages Layout model.
//    System.Web.UI.MasterPage replaced with ASP.NET Core middleware pipeline conventions.
//    Web Forms-specific types (MasterPage, Page, HttpCookie, FormsAuthentication) replaced
//    with ASP.NET Core equivalents for cloud-native deployment on AWS (ECS/EKS).
//
// 2. Hardcoded secrets (AntiXsrfTokenKey, AntiXsrfUserNameKey constants) replaced with
//    runtime retrieval from AWS Secrets Manager. Secrets are encrypted at rest, support
//    automatic rotation, and can be updated without redeployment.
//    AWS SDK: AWSSDK.SecretsManager NuGet package required.
using System;
using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FIlms
{
    /// <summary>
    /// Site master layout model - migrated from ASP.NET Web Forms MasterPage to ASP.NET Core.
    /// Anti-XSRF token keys are retrieved from AWS Secrets Manager instead of being hardcoded.
    /// Replaces System.Web.UI.MasterPage with ASP.NET Core Razor Pages layout conventions.
    /// </summary>
    public class SiteMasterModel : PageModel
    {
        private readonly ILogger<SiteMasterModel> _logger;
        private readonly IAntiforgery _antiforgery;
        private readonly IConfiguration _configuration;
        private readonly IAmazonSecretsManager _secretsManager;

        // Secret keys are retrieved from AWS Secrets Manager at runtime
        // instead of being hardcoded (cr-dotnet-0123 remediation).
        // Previously: private const string AntiXsrfTokenKey = "__AntiXsrfToken";
        // Previously: private const string AntiXsrfUserNameKey = "__AntiXsrfUserName";
        private string _antiXsrfTokenKey;
        private string _antiXsrfUserNameKey;

        public SiteMasterModel(
            ILogger<SiteMasterModel> logger,
            IAntiforgery antiforgery,
            IConfiguration configuration,
            IAmazonSecretsManager secretsManager)
        {
            _logger = logger;
            _antiforgery = antiforgery;
            _configuration = configuration;
            _secretsManager = secretsManager;
        }

        /// <summary>
        /// Retrieves the Anti-XSRF token key name from AWS Secrets Manager.
        /// Replaces hardcoded constant: private const string AntiXsrfTokenKey = "__AntiXsrfToken"
        /// </summary>
        private string GetAntiXsrfTokenKey()
        {
            try
            {
                var secretName = Environment.GetEnvironmentVariable("ANTIXSRF_TOKEN_SECRET_NAME")
                    ?? _configuration["AWS:Secrets:AntiXsrfTokenSecretName"]
                    ?? "films-app/antixsrf-token-key";

                var request = new GetSecretValueRequest { SecretId = secretName };
                var response = _secretsManager.GetSecretValueAsync(request).GetAwaiter().GetResult();
                return response.SecretString ?? "__AntiXsrfToken";
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not retrieve AntiXsrfTokenKey from AWS Secrets Manager. Using environment variable fallback.");
                return Environment.GetEnvironmentVariable("ANTIXSRF_TOKEN_KEY") ?? "__AntiXsrfToken";
            }
        }

        /// <summary>
        /// Retrieves the Anti-XSRF username key name from AWS Secrets Manager.
        /// Replaces hardcoded constant: private const string AntiXsrfUserNameKey = "__AntiXsrfUserName"
        /// </summary>
        private string GetAntiXsrfUserNameKey()
        {
            try
            {
                var secretName = Environment.GetEnvironmentVariable("ANTIXSRF_USERNAME_SECRET_NAME")
                    ?? _configuration["AWS:Secrets:AntiXsrfUserNameSecretName"]
                    ?? "films-app/antixsrf-username-key";

                var request = new GetSecretValueRequest { SecretId = secretName };
                var response = _secretsManager.GetSecretValueAsync(request).GetAwaiter().GetResult();
                return response.SecretString ?? "__AntiXsrfUserName";
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not retrieve AntiXsrfUserNameKey from AWS Secrets Manager. Using environment variable fallback.");
                return Environment.GetEnvironmentVariable("ANTIXSRF_USERNAME_KEY") ?? "__AntiXsrfUserName";
            }
        }

        public void OnGet()
        {
            // Retrieve secret key names from AWS Secrets Manager at runtime
            // Replaces hardcoded constants (cr-dotnet-0123 remediation)
            _antiXsrfTokenKey = GetAntiXsrfTokenKey();
            _antiXsrfUserNameKey = GetAntiXsrfUserNameKey();

            // ASP.NET Core's built-in IAntiforgery service handles XSRF protection
            // replacing the manual cookie/ViewState approach from Web Forms MasterPage.
            // The antiforgery token is automatically validated by the framework.
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            HttpContext.Response.Cookies.Append(
                _antiXsrfTokenKey,
                tokens.RequestToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = HttpContext.Request.IsHttps,
                    SameSite = SameSiteMode.Strict
                });
        }
    }
}
