namespace Test_Taste_Console_Application.Constants
{
    public static class HttpClientSettings
    {
        public const string JsonType = "application/json";
        public const string BearerScheme = "Bearer";

        //The configuration key (appsettings.json: { "Api": { "Key": "..." } }).
        //It can also be set with the environment variable Api__Key.
        public const string ApiKeyConfigurationKey = "Api:Key";
        public const string ApiKeyPlaceholder = "<YOUR_API_KEY>";
    }
}
