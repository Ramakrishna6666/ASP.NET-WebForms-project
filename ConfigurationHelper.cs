using System;
using System.Configuration;

namespace FIlms
{
    /// <summary>
    /// Configuration helper to read settings from environment variables or Web.config
    /// This supports containerization by allowing runtime configuration through environment variables
    /// </summary>
    public static class ConfigurationHelper
    {
        /// <summary>
        /// Gets a configuration value, first checking environment variables, then falling back to Web.config
        /// </summary>
        /// <param name="key">Configuration key</param>
        /// <param name="defaultValue">Default value if not found</param>
        /// <returns>Configuration value</returns>
        public static string GetConfigValue(string key, string defaultValue = null)
        {
            // First check environment variables (for containerized deployments)
            string envValue = Environment.GetEnvironmentVariable(key);
            if (!string.IsNullOrEmpty(envValue))
            {
                return envValue;
            }

            // Fall back to Web.config appSettings
            string configValue = ConfigurationManager.AppSettings[key];
            if (!string.IsNullOrEmpty(configValue))
            {
                return configValue;
            }

            return defaultValue;
        }

        /// <summary>
        /// Gets a file path from configuration, ensuring it's Linux-compatible
        /// </summary>
        /// <param name="key">Configuration key for the file path</param>
        /// <param name="defaultPath">Default path if not configured</param>
        /// <returns>Configured file path</returns>
        public static string GetFilePath(string key, string defaultPath = null)
        {
            string path = GetConfigValue(key, defaultPath);
            
            // Ensure path is not null
            if (string.IsNullOrEmpty(path))
            {
                throw new ConfigurationErrorsException($"File path configuration '{key}' is not set. Please configure it via environment variable or Web.config.");
            }

            // Normalize path separators for cross-platform compatibility
            // In Linux containers, use forward slashes
            if (Environment.OSVersion.Platform == PlatformID.Unix || 
                Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                path = path.Replace('\\', '/');
            }

            return path;
        }

        /// <summary>
        /// Gets a connection string, first checking environment variables, then Web.config
        /// </summary>
        /// <param name="name">Connection string name</param>
        /// <returns>Connection string</returns>
        public static string GetConnectionString(string name)
        {
            // Check environment variable first (for containerized deployments)
            string envValue = Environment.GetEnvironmentVariable($"ConnectionStrings__{name}");
            if (!string.IsNullOrEmpty(envValue))
            {
                return envValue;
            }

            // Fall back to Web.config
            var connectionString = ConfigurationManager.ConnectionStrings[name];
            if (connectionString != null)
            {
                return connectionString.ConnectionString;
            }

            throw new ConfigurationErrorsException($"Connection string '{name}' not found in configuration.");
        }
    }
}
