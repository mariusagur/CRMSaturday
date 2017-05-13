﻿using Dyn365PluginsAndWorkflows.Models;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace Dyn365PluginsAndWorkflows.Plugins
{
    /// <summary>
    /// This code is copied from Jason Lattimer
    /// https://github.com/jlattimer/CrmAzureKeyVaultExample
    /// </summary>
    public class ConnectToKeyVault : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            string clientId = "00000000-0000-0000-0000-000000000000";
            string clientSecret = "00000000000000000000000000000000000000000000";
            string tenantId = "00000000-0000-0000-0000-000000000000";

            string secretUrl = "https://msdyn365.vault.azure.net/secrets/MyPassword/00000000000000000000000000000000";

            //Retrieve the access token required for authentication
            var getTokenTask = Task.Run(async () => await GetToken(clientId, clientSecret, tenantId));
            Task.WaitAll(getTokenTask);
            if (getTokenTask.Result == null)
                throw new InvalidPluginExecutionException("Error retrieving access token");
            //Deserialize the token response to get the access token
            TokenResponse tokenResponse = DeserializeResponse<TokenResponse>(getTokenTask.Result);
            string token = tokenResponse.access_token;


            //Retrieve a secret by its url
            var getKeyByUrlTask1 = Task.Run(async () => await GetSecretByUrl(token, secretUrl));
            Task.WaitAll(getKeyByUrlTask1);
            if (getKeyByUrlTask1.Result == null)
                throw new InvalidPluginExecutionException("Error retrieving secret value from key vault");
            //Deserialize the vault response to get the secret
            GetSecretResponse getSecretResponse1 = DeserializeResponse<GetSecretResponse>(getKeyByUrlTask1.Result);
            //returnedValue is the Azure Key Vault Secret
            string returnedValue = getSecretResponse1.value;


            string vaultName = "https://msdyn365hub.vault.azure.net";
            string secretName = "MyPassword";

            //Retrieve the latest version of a secret by name
            var getKeyByNameTask = Task.Run(async () => await GetSecretByName(token, vaultName, secretName));
            Task.WaitAll(getKeyByNameTask);
            if (getKeyByNameTask.Result == null)
                throw new InvalidPluginExecutionException("Error retrieving secret versions from key vault");
            var retrievedSecretUrl = getKeyByNameTask.Result;
            // Retrieve a secret by its url
            var getKeyByUrlTask2 = Task.Run(async () => await GetSecretByUrl(token, retrievedSecretUrl));
            Task.WaitAll(getKeyByUrlTask2);
            if (getKeyByUrlTask2.Result == null)
                throw new InvalidPluginExecutionException("Error retrieving secret value from key vault");
            //Deserialize the vault response to get the secret
            GetSecretResponse getSecretResponse2 = DeserializeResponse<GetSecretResponse>(getKeyByUrlTask2.Result);
            //returnedValue is the Azure Key Vault Secret
            string returnedValue2 = getSecretResponse2.value;
        }

        //Get the access token required to access the Key Vault
        private async Task<string> GetToken(string clientId, string clientSecret, string tenantId)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("resource", "https://msdyn365hub.azure.net"),
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

        //Get the Secret value from the Key Vault by url - api-version is required
        private async Task<string> GetSecretByUrl(string token, string secretUrl)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get,
                        new Uri(secretUrl + "?api-version=2016-10-01"));
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await httpClient.SendAsync(request);

                return !response.IsSuccessStatusCode ? null
                    : response.Content.ReadAsStringAsync().Result;
            }
        }

        //Get the most recent, enabled Secret value by name  - api-version is required
        private async Task<string> GetSecretByName(string token, string vaultName, string secretName)
        {
            string nextLink = vaultName + "/secrets/" + secretName + "/versions?api-version=2016-10-01";
            List<Value> values = new List<Value>();

            using (HttpClient httpClient = new HttpClient())
            {
                while (!string.IsNullOrEmpty(nextLink))
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get,
                        new Uri(nextLink));
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    HttpResponseMessage response = await httpClient.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                        return null;

                    var versions = DeserializeResponse<GetSecretVersionsResponse>(response.Content.ReadAsStringAsync().Result);
                    values.AddRange(versions.value);
                    nextLink = versions.nextLink;
                }
            }

            Value mostRecentValue =
                values.Where(a => a.attributes.enabled)
                    .OrderByDescending(a => UnixTimeToUtc(a.attributes.created))
                    .FirstOrDefault();

            return mostRecentValue?.id;
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

        private DateTime UnixTimeToUtc(double unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var timeSpan = TimeSpan.FromSeconds(unixTime);
            return epoch.Add(timeSpan).ToUniversalTime();
        }
    }
}
