namespace Test_Taste_Console_Application.Constants
{
    public class OutputString
    {
        public const string PlanetNumber = "Planet's Number";
        public const string PlanetSemiMajorAxis = "Planet's Semi-Major Axis";
        public const string PlanetId = "Planet's Id";
        public const string TotalMoons = "Total Moons";
        public const string MoonNumber = "Moon's Number";
        public const string MoonId = "Moon's Id";
        public const string MoonMassExponent = "Moon's Mass Exponent";
        public const string MoonMassValue = "Moon's Mass Value";
        public const string PlanetMoonAverageTemperature = "Average Moon Temperature";
        public const string MoonsWithTemperature = "Moons With Temperature";
        public const string NoMoonsFound = "No moons found.";
        public const string NoPlanetsFound = "No planets found.";
        public const string NoPlanetsWithMoonsFound = "No planets with moons found.";
        public const string FileCreated = "The file is created in: ";
        public const string FolderCreated = "The folder is created in: ";

        //(The JSON response itself only contains the number, the unit comes from the API documentation.)
        public const string TemperatureUnit = "K";
        public const string NotAvailable = "n/a";

        //The steps the application is in.
        public const string LoadingPlanets = "Loading planets and their moons (the temperature of every moon is requested, this can take a while)...";
        public const string LoadingMoons = "Loading moons...";
        public const string Calculating = "Calculating the average temperatures...";
        public const string WritingData = "Writing data...";
        public const string StepFailed = "This step failed, the next step is started.";
    }
}
