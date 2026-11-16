using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Test_Taste_Console_Application.Constants;

namespace Test_Taste_Console_Application.Domain.Services
{
    ///<summary>
    /// A service to create the HttpClient.
    ///</summary>
    public class HttpClientService
    {
        public HttpClient Client { get; }

        public HttpClientService(HttpClient client, IConfiguration configuration)
        {
            //The API key is read from the configuration (appsettings.json).
            var apiKey = configuration[HttpClientSettings.ApiKeyConfigurationKey];
            if (string.IsNullOrWhiteSpace(apiKey) || apiKey == HttpClientSettings.ApiKeyPlaceholder)
            {
                throw new InvalidOperationException(ExceptionMessage.ApiKeyMissing);
            }

            //The HTTP client is configured in the constructor.
            Client = client;
            Client.BaseAddress = new Uri(UriPath.BaseUri);
            Client.DefaultRequestHeaders.Accept.Add(new
                MediaTypeWithQualityHeaderValue(HttpClientSettings.JsonType));

            //Every request sends the header "Authorization: Bearer <API key>".
            Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(HttpClientSettings.BearerScheme, apiKey.Trim());
        }
    }
}
