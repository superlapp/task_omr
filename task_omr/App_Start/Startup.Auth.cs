using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using task_omr.Models;

namespace task_omr
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {

            //var configuration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            //var connectionString = configuration.ConnectionStrings.ConnectionStrings["ConnStringAlias"].ConnectionString;
            //if (!connectionString.Contains("multipleactiveresultsets=True;"))
            //{
            //    connectionString = connectionString.TrimEnd('\'');
            //    connectionString = connectionString += "multipleactiveresultsets=True;\'";
            //    configuration.ConnectionStrings.ConnectionStrings["ConnStringAlias"].ConnectionString = connectionString;
            //    configuration.Save();
            //}

            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });            
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            //-----------------------------------------------------------------External authentication
            app.UseMicrosoftAccountAuthentication(
                clientId: "7692af49-63de-489e-82a7-b239e5a0bfaa",
                clientSecret: "n7L64onOqk0jPKvH5Y3hV7P");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            app.UseFacebookAuthentication(
                appId: "199232967148926",
                appSecret: "035472e466234b9f501b723eef2da51d");

            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "705472391733-1imt0q8kv7i71lmem20vnvpb24hg8agf.apps.googleusercontent.com",
                ClientSecret = "qlpHZ2O50yypI_lPhULubrkr"
            });
        }
    }
}