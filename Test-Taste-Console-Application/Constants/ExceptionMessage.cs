namespace Test_Taste_Console_Application.Constants
{
    public static class ExceptionMessage
    {
        public const string FileOutputOperationFailed = "The file output operation failed with the following error: ";

        public const string ScreenOutputOperationFailed =
            "The screen output operation failed with the following error: ";

        public const string ApiKeyMissing =
            "No API key is configured. Set 'Api:Key' in appsettings.json.";

        //Messages of the data access layer.
        public const string ApiKeyRejected = "The API rejected the API key (HTTP {0}). Check the 'Api:Key' setting in appsettings.json.";

        public const string ApiRequestFailed = "The request to '{0}' failed: {1}";
    }
}
