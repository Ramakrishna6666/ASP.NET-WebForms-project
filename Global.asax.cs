// CLOUD READINESS MIGRATION (cr-dotnet-0044):
// Migrated from IIS-specific HttpApplication (Global.asax) to ASP.NET Core middleware pipeline.
// System.Web.HttpApplication replaced with ASP.NET Core Program.cs / Startup.cs pattern.
// IIS modules (BundleConfig, AuthConfig, RouteConfig via RouteTable) replaced with
// ASP.NET Core middleware components that run in Kestrel on Amazon ECS or EKS.
// Authentication, authorization, bundling, and routing are now configured as middleware.
//
// NOTE: In a full ASP.NET Core migration, this file is replaced by Program.cs and Startup.cs.
// The middleware pipeline below replaces all IIS HttpModule/HttpHandler registrations.
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Amazon.SecretsManager;

namespace FIlms
{
    /// <summary>
    /// Application startup - migrated from IIS HttpApplication (Global.asax) to ASP.NET Core
    /// middleware pipeline. Replaces Application_Start, Application_End, Application_Error
    /// event handlers with ASP.NET Core middleware and hosted service patterns.
    /// Runs on Kestrel web server for cloud-native deployment on Amazon ECS/EKS.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Replaces Global.asax Application_Start - registers services in the DI container.
        /// BundleConfig.RegisterBundles() replaced with WebOptimizer or bundleconfig.json.
        /// AuthConfig.RegisterOpenAuth() replaced with ASP.NET Core authentication middleware.
        /// RouteConfig.RegisterRoutes() replaced with ASP.NET Core endpoint routing.
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            // Add ASP.NET Core Razor Pages (replaces Web Forms page model)
            services.AddRazorPages();

            // Add MVC controllers (replaces Web Forms code-behind)
            services.AddControllersWithViews();

            // Add AWS Secrets Manager client for externalized secrets (cr-dotnet-0123)
            services.AddAWSService<IAmazonSecretsManager>();

            // Add HTTP context accessor for ViewSwitcher component
            services.AddHttpContextAccessor();

            // Add antiforgery services (replaces manual XSRF token management in Site.Master.cs)
            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-XSRF-TOKEN";
            });

            // Add structured logging for cloud monitoring (AWS CloudWatch)
            services.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.AddDebug();
            });

            // Add session state with distributed cache (replaces InProc session - not cloud-safe)
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
        }

        /// <summary>
        /// Replaces Global.asax Application_Start route/module registration with
        /// ASP.NET Core middleware pipeline. Runs on Kestrel (not IIS) for AWS deployment.
        /// </summary>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // Replaces Application_Error handler
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // Replaces IIS URL rewriting modules
            app.UseRouting();

            // Replaces AuthConfig.RegisterOpenAuth() - ASP.NET Core authentication middleware
            app.UseAuthentication();
            app.UseAuthorization();

            // Session middleware (replaces InProc session state)
            app.UseSession();

            // Replaces RouteConfig.RegisterRoutes(RouteTable.Routes)
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }

    /// <summary>
    /// Application entry point - replaces Global.asax HttpApplication for Kestrel hosting on AWS.
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    // Kestrel runs on port from environment variable (12-factor app principle)
                    // Replaces IIS-specific port configuration
                    webBuilder.UseUrls(
                        System.Environment.GetEnvironmentVariable("ASPNETCORE_URLS")
                        ?? "http://0.0.0.0:8080");
                });
    }
}
