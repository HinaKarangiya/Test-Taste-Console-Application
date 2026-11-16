using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Test_Taste_Console_Application.Constants;
using Test_Taste_Console_Application.Domain.DataTransferObjects;
using Test_Taste_Console_Application.Domain.DataTransferObjects.JsonObjects;
using Test_Taste_Console_Application.Domain.Services.Interfaces;
using Test_Taste_Console_Application.Utilities;

namespace Test_Taste_Console_Application.Domain.Services
{
    public class SolarSystemService : ISolarSystemService
    {
        private readonly BaseHttpClient _baseHttpClient;

        public SolarSystemService(BaseHttpClient baseHttpClient)
        {
            _baseHttpClient = baseHttpClient;
        }

        public Task<IReadOnlyList<PlanetDto>> GetPlanetsAsync()
        {
            return GetAllPagesAsync<PlanetDto>(UriPath.GetAllPlanetsWithMoonsQueryParameters);
        }

        public Task<IReadOnlyList<MoonDto>> GetMoonsWithMassAsync()
        {
            return GetAllPagesAsync<MoonDto>(UriPath.GetAllMoonsWithMassQueryParameters);
        }

        public Task<MoonDto> GetMoonByIdAsync(string id)
        {
            return GetAsync<MoonDto>(UriPath.GetMoonByIdQueryParameters + Uri.EscapeDataString(id));
        }

        /// Requests a list page by page and returns all the items together.
        /// A page that has fewer items than the page size is the last page.
        private async Task<IReadOnlyList<T>> GetAllPagesAsync<T>(string query)
        {
            var allItems = new List<T>();

            for (var page = 1; page <= HttpClientSettings.MaxPages; page++)
            {
                var pagingParameters = string.Format(CultureInfo.InvariantCulture, UriPath.PagingQueryParameters,
                    page, HttpClientSettings.PageSize);

                var result = await GetAsync<JsonResult<T>>(query + pagingParameters);
                var items = result.Bodies;

                //An empty (or missing) page means that there is nothing left.
                if (items == null || items.Count == 0) return allItems;

                allItems.AddRange(items);
                Logger.Instance.Info(string.Format(LoggerMessage.PageLoaded, page, items.Count));

                if (items.Count != HttpClientSettings.PageSize) return allItems;
            }

            Logger.Instance.Warn(string.Format(LoggerMessage.MaxPagesReached, HttpClientSettings.MaxPages, query));
            return allItems;
        }

        /// Sends one GET request and turns every kind of failure into an <see cref="HttpRequestException"/>
        /// with a message that makes sense for the user.
        private async Task<T> GetAsync<T>(string relativeUri) where T : class
        {
            HttpResponseMessage response;
            try
            {
                response = await _baseHttpClient.Client.GetAsync(relativeUri);
            }
            catch (TaskCanceledException exception)
            {
                //HttpClient reports a timeout as a cancelled task.
                throw CreateException(relativeUri, "the request timed out", exception);
            }
            catch (HttpRequestException exception)
            {
                throw CreateException(relativeUri, exception.Message, exception);
            }

            using (response)
            {
                if (!response.IsSuccessStatusCode)
                {
                    Logger.Instance.Warn($"{LoggerMessage.GetRequestFailed}{response.StatusCode}");

                    //A wrong API key has its own message, every other status uses the general message.
                    var wrongKey = response.StatusCode == HttpStatusCode.Unauthorized
                                   || response.StatusCode == HttpStatusCode.Forbidden;
                    var message = wrongKey
                        ? string.Format(ExceptionMessage.ApiKeyRejected, (int)response.StatusCode)
                        : string.Format(ExceptionMessage.ApiRequestFailed, relativeUri,
                            $"HTTP {(int)response.StatusCode} ({response.StatusCode})");
                    throw new HttpRequestException(message, null, response.StatusCode);
                }

                var content = await response.Content.ReadAsStringAsync();

                T result;
                try
                {
                    result = JsonConvert.DeserializeObject<T>(content);
                }
                catch (JsonException exception)
                {
                    throw CreateException(relativeUri, $"the response is not valid JSON ({exception.Message})", exception);
                }

                if (result == null)
                {
                    throw CreateException(relativeUri, "the response is empty", null);
                }

                return result;
            }
        }

        //Every failure that isn't a wrong API key uses the same general message with its own reason.
        private static HttpRequestException CreateException(string relativeUri, string reason, Exception innerException)
        {
            return new HttpRequestException(string.Format(ExceptionMessage.ApiRequestFailed, relativeUri, reason),
                innerException);
        }
    }
}
