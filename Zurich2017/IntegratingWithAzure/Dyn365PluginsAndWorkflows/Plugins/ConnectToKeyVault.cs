using Dyn365PluginsAndWorkflows.Models;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Dyn365PluginsAndWorkflows.Plugins
{
    public class ConnectToKeyVault : IPlugin
    {
        string clientId = "00000000-0000-0000-0000-000000000000";
        string clientSecret = "00000000000000000000000000000000000000000000";
        string tenantId = "00000000-0000-0000-0000-000000000000";
        public void Execute(IServiceProvider serviceProvider)
        {

            //Retrieve the access token required for authentication
            var getTokenTask = Task.Run(async () => await GetToken(clientId, clientSecret, tenantId));
            Task.WaitAll(getTokenTask);
            if (getTokenTask.Result == null)
                throw new InvalidPluginExecutionException("Error retrieving access token");
            //Deserialize the token response to get the access token
            TokenResponse tokenResponse = DeserializeResponse<TokenResponse>(getTokenTask.Result);
            string token = tokenResponse.access_token;

        }
        //Get the access token required to access the Key Vault
        private async Task<string> GetToken(string clientId, string clientSecret, string tenantId)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var formContent = new FormUrlEncodedContent(new[]
                {
            new KeyValuePair<string, string>("resource", "https://vault.azure.net"),
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("client_secret", clientSecret),
            new KeyValuePair<string, string>("grant_type", "client_credentials")
        });

                HttpResponseMessage response = await httpClient.PostAsync(
                    "https://login.windows.net/" + tenantId + "/oauth2/token", formContent);

                return !response.IsSuccessStatusCode ? null
                    : response.Content.ReadAsStringAsync().Result;
            }
        }

        //Generic JSON to object deserialization 
        private T DeserializeResponse<T>(string response)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                StreamWriter writer = new StreamWriter(stream);
                writer.Write(response);
                writer.Flush();
                stream.Position = 0;
                T responseObject = (T)serializer.ReadObject(stream);
                return responseObject;
            }
        }
    }
}
