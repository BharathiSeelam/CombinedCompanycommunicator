
namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.Teams
{
    using Microsoft.Graph;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    public class TokenGeneratorExtension
    {
        public GraphServiceClient GenerateGraphClient(string clientID, string scope, string clientSecret, string grantType, string TenantId)
        {
            GraphServiceClient graphClient = new GraphServiceClient(
               new DelegateAuthenticationProvider(
                   async (requestMessage) =>
                   {
                       // Configure the permissions
                       String[] scopes = {
                        "User.Read",
                       };
                       TokenResponses tokenResponse = this.GetTokenAsync(clientID, scope, clientSecret, grantType, TenantId).Result;
                       requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", tokenResponse.access_token);
                   }));
            return graphClient;
        }

        public async Task<TokenResponses> GetTokenAsync(string clientID, string scope, string clientSecret, string grantType, string TenantId)
        {
            string uri = string.Format("https://login.microsoftonline.com/{0}/oauth2/v2.0/token", TenantId);
            FormUrlEncodedContent data = new FormUrlEncodedContent(new Dictionary<string, string> {
                {"Content-Type","application/x-www-form-urlencoded"},
                {"client_id", clientID},
                {"scope", scope },
                { "client_secret", clientSecret },
                { "grant_type", grantType },
            });
            string content = string.Empty;
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    HttpResponseMessage response = await httpClient.PostAsync(uri, data);
                    response.EnsureSuccessStatusCode();
                    content = await response.Content.ReadAsStringAsync();
                    return await Task.Run(() => JsonConvert.DeserializeObject<TokenResponses>(content));
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

    }

    public class TokenResponses
    {
        public string token_type { get; set; }
        public string expires_in { get; set; }
        public string ext_expires_in { get; set; }
        public string access_token { get; set; }
    }

}
