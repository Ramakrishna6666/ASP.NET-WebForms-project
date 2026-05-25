using System;
using System.Web;
using System.Web.UI;

namespace FIlms
{
    /// <summary>
    /// Health check endpoint for container orchestration (ECS/EKS)
    /// Returns HTTP 200 with JSON status when application is healthy
    /// </summary>
    public partial class HealthCheck : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Clear any default response
            Response.Clear();
            Response.ContentType = "application/json";
            
            try
            {
                // Perform basic health checks
                bool isHealthy = PerformHealthChecks();
                
                if (isHealthy)
                {
                    Response.StatusCode = 200;
                    Response.Write("{\"status\":\"healthy\",\"timestamp\":\"" + DateTime.UtcNow.ToString("o") + "\"}");
                }
                else
                {
                    Response.StatusCode = 503;
                    Response.Write("{\"status\":\"unhealthy\",\"timestamp\":\"" + DateTime.UtcNow.ToString("o") + "\"}");
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = 503;
                Response.Write("{\"status\":\"unhealthy\",\"error\":\"" + ex.Message.Replace("\"", "'") + "\",\"timestamp\":\"" + DateTime.UtcNow.ToString("o") + "\"}");
            }
            
            Response.End();
        }
        
        /// <summary>
        /// Performs application health checks
        /// </summary>
        /// <returns>True if healthy, false otherwise</returns>
        private bool PerformHealthChecks()
        {
            // Basic health check - application is running
            // Add additional checks as needed:
            // - Database connectivity
            // - External service availability
            // - File system access
            
            // For now, return true if we can execute this code
            return true;
        }
    }
}
