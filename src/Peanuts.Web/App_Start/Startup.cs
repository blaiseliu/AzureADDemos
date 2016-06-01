using System.Configuration;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;

[assembly: OwinStartup(typeof(Peanuts.Web.Startup))]
namespace Peanuts.Web
{
    public class Startup
    {

        private static string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private static string azureADInstance = ConfigurationManager.AppSettings["ida:AzureADInstance"];
        private static string tenant = ConfigurationManager.AppSettings["ida:tenant"];
        private static string postLogoutRedirectUrl = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUrl"];

        string authority = string.Format(CultureInfo.InvariantCulture, azureADInstance, tenant);

        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = clientId,
                    Authority = authority,
                    PostLogoutRedirectUri = postLogoutRedirectUrl,
                    Notifications = new OpenIdConnectAuthenticationNotifications
                    {
                        AuthenticationFailed = x =>
                      {
                          x.HandleResponse();
                          x.Response.Redirect("/Error/Messges=" + x.Exception.Message);
                          return Task.FromResult(0);
                      }
                    }
                });
        }
    }
}