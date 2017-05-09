using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Configuration;
using System.Security.Claims;

namespace IntegrationWebService.Factories
{
    public static class AuthFactory
    {
        public static AuthenticationContext GetAuthContext()
        {
            var tenantId = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
            return new AuthenticationContext(ConfigurationManager.AppSettings["ida:AADInstance"] + tenantId);
        }

        public static ClientCredential GetCredentials()
        {
            return new ClientCredential(ConfigurationManager.AppSettings["ida:ClientId"], ConfigurationManager.AppSettings["ida:ClientSecret"]);
        }
    }
}