namespace Test_Taste_Console_Application.Constants
{
    public static class HttpClientSettings
    {
        public const string JsonType = "application/json";
        public const string BearerScheme = "Bearer";

        //The configuration key (appsettings.json: { "Api": { "Key": "..." } }).
        public const string ApiKeyConfigurationKey = "Api:Key";
        public const string ApiKeyPlaceholder = "<YOUR_API_KEY>";

        //Paging: a list is requested page by page until a page is not full anymore.
        public const int PageSize = 100;
        public const int MaxPages = 100;
        public const int MaxParallelRequests = 10;
    }
}
