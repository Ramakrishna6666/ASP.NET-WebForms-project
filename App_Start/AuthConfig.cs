using System;
using Microsoft.AspNet.Membership.OpenAuth;

namespace FIlms
{
    /// <summary>
    /// Configuration for authentication and OAuth providers
    /// </summary>
    public static class AuthConfig
    {
        /// <summary>
        /// Registers OAuth authentication providers
        /// </summary>
        public static void RegisterOpenAuth()
        {
            // To let users of this site log in using their accounts from other sites such as Microsoft, Facebook, and Twitter,
            // you must update this site. For more information visit http://go.microsoft.com/fwlink/?LinkID=252166

            // Uncomment and configure OAuth providers as needed:
            
            // Microsoft
            // OAuthWebSecurity.RegisterMicrosoftClient(
            //     clientId: "",
            //     clientSecret: "");

            // Twitter
            // OAuthWebSecurity.RegisterTwitterClient(
            //     consumerKey: "",
            //     consumerSecret: "");

            // Facebook
            // OAuthWebSecurity.RegisterFacebookClient(
            //     appId: "",
            //     appSecret: "");

            // Google
            // OAuthWebSecurity.RegisterGoogleClient();
        }
    }
}
