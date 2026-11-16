namespace Test_Taste_Console_Application.Constants
{
    public static class LoggerMessage
    {
        public const string GetRequestFailed = "Get request failed with the code: ";

        public const string ScreenOutputOperationFailed =
            "The screen output operation failed with the following error: ";

        public const string FileOutputOperationFailed = "The file output operation failed with the following error: ";

        public const string PageLoaded = "Loaded page {0} ({1} items).";
        public const string MaxPagesReached = "The maximum of {0} pages is reached for '{1}', the result can be incomplete.";
        public const string MoonDetailsMissing = "The details of the moon '{0}' can't be loaded, it is shown without temperature.";
        public const string MoonDetailsLoading = "Loading the details of {0} moons...";
        public const string MoonDetailsLoaded = "Loaded the details of {0} moons.";
    }
}
