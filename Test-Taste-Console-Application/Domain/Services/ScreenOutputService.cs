using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Test_Taste_Console_Application.Constants;
using Test_Taste_Console_Application.Domain.Objects;
using Test_Taste_Console_Application.Domain.Services.Interfaces;
using Test_Taste_Console_Application.Utilities;

namespace Test_Taste_Console_Application.Domain.Services
{
    /// <inheritdoc />
    public class ScreenOutputService : IOutputService
    {
        private readonly IPlanetService _planetService;

        private readonly IMoonService _moonService;

        public ScreenOutputService(IPlanetService planetService, IMoonService moonService)
        {
            _planetService = planetService;
            _moonService = moonService;
        }

        public async Task OutputAllPlanetsAndTheirMoonsToConsoleAsync()
        {
            //The service gets all the planets from the API.
            ConsoleWriter.WriteStatus(OutputString.LoadingPlanets);
            var planets = (await _planetService.GetAllPlanetsAsync()).ToArray();

            //If the planets aren't found, then the function stops and tells that to the user via the console.
            if (!planets.Any())
            {
                Console.WriteLine(OutputString.NoPlanetsFound);
                return;
            }

            ConsoleWriter.WriteStatus(OutputString.WritingData);

            //The column sizes and labels for the planets are configured here.
            var columnSizesForPlanets = new[] { 20, 20, 30, 20 };
            var columnLabelsForPlanets = new[]
            {
                OutputString.PlanetNumber, OutputString.PlanetId, OutputString.PlanetSemiMajorAxis,
                OutputString.TotalMoons
            };

            //The column sizes and labels for the moons are configured here.
            //The second moon's column needs the 2 extra '-' characters so that it's aligned with the planet's column.
            var columnSizesForMoons = new[] { 20, 70 + 2 };
            var columnLabelsForMoons = new[]
            {
                OutputString.MoonNumber, OutputString.MoonId
            };

            //The for loop creates the correct output.
            for (int i = 0, j = 1; i < planets.Length; i++, j++)
            {
                //First the line is created.
                ConsoleWriter.CreateLine(columnSizesForPlanets);

                //Under the line the header is created.
                ConsoleWriter.CreateText(columnLabelsForPlanets, columnSizesForPlanets);

                //Under the header the planet data is shown.
                ConsoleWriter.CreateText(
                    new[]
                    {
                        j.ToString(), CultureInfoUtility.TextInfo.ToTitleCase(planets[i].Id),
                        planets[i].SemiMajorAxis.ToString(),
                        planets[i].Moons.Count.ToString()
                    },
                    columnSizesForPlanets);

                //Under the planet data the header for the moons is created.
                ConsoleWriter.CreateLine(columnSizesForPlanets);
                ConsoleWriter.CreateText(columnLabelsForMoons, columnSizesForMoons);

                //The for loop creates the correct output.
                for (int k = 0, l = 1; k < planets[i].Moons.Count; k++, l++)
                {
                    ConsoleWriter.CreateText(
                        new[]
                        {
                            l.ToString(), CultureInfoUtility.TextInfo.ToTitleCase(planets[i].Moons.ElementAt(k).Id)
                        },
                        columnSizesForMoons);
                }

                //Under the data the footer is created.
                ConsoleWriter.CreateLine(columnSizesForMoons);
                ConsoleWriter.CreateEmptyLines(2);

                /*
                    This is an example of the output for the planet Earth:
                    --------------------+--------------------+------------------------------+--------------------
                    Planet's Number     |Planet's Id         |Planet's Semi-Major Axis      |Total Moons
                    10                  |Terre               |0                             |1
                    --------------------+--------------------+------------------------------+--------------------
                    Moon's Number       |Moon's Id
                    1                   |La Lune
                    --------------------+------------------------------------------------------------------------
                */
            }
        }

        public async Task OutputAllMoonsAndTheirMassToConsoleAsync()
        {
            //The function works the same way as the OutputAllPlanetsAndTheirMoonsToConsoleAsync function. You can find more comments there.
            ConsoleWriter.WriteStatus(OutputString.LoadingMoons);
            var moons = (await _moonService.GetAllMoonsAsync()).ToArray();

            if (!moons.Any())
            {
                Console.WriteLine(OutputString.NoMoonsFound);
                return;
            }

            ConsoleWriter.WriteStatus(OutputString.WritingData);

            var columnSizesForMoons = new[] { 20, 20, 30, 20 };
            var columnLabelsForMoons = new[]
            {
                OutputString.MoonNumber, OutputString.MoonId, OutputString.MoonMassExponent, OutputString.MoonMassValue
            };

            ConsoleWriter.CreateHeader(columnLabelsForMoons, columnSizesForMoons);

            for (int i = 0, j = 1; i < moons.Length; i++, j++)
            {
                ConsoleWriter.CreateText(
                    new[]
                    {
                        j.ToString(), CultureInfoUtility.TextInfo.ToTitleCase(moons[i].Id),
                        moons[i].MassExponent.ToString(), moons[i].MassValue.ToString()
                    },
                    columnSizesForMoons);
            }

            ConsoleWriter.CreateLine(columnSizesForMoons);
            ConsoleWriter.CreateEmptyLines(2);

            /*
                This is an example of the output for the moon around the earth:
                --------------------+--------------------+------------------------------+--------------------
                Moon's Number       |Moon's Id           |Moon's Mass Exponent          |Moon's Mass Value
                --------------------+--------------------+------------------------------+--------------------
                1                   |Lune                |22                            |7,346
                ...more data...
                --------------------+--------------------+------------------------------+--------------------
             */
        }

        public async Task OutputAllPlanetsWithMoonsAndTheirAverageMoonTemperatureToConsoleAsync()
        {
            //The function works the same way as the OutputAllPlanetsAndTheirMoonsToConsoleAsync function. You can find more comments there.
            ConsoleWriter.WriteStatus(OutputString.LoadingPlanets);
            var allPlanets = await _planetService.GetAllPlanetsAsync();

            //Only the planets that have at least one moon are listed.
            var planets = allPlanets.Where(planet => planet.HasMoons()).ToArray();
            if (!planets.Any())
            {
                Console.WriteLine(OutputString.NoPlanetsWithMoonsFound);
                return;
            }

            ConsoleWriter.WriteStatus(OutputString.Calculating);

            //The average is calculated by the domain (Planet): the average of the temperatures that the API returned
            //for the moons of the planet (also when the API returned 0).
            ConsoleWriter.WriteStatus(OutputString.WritingData);

            var columnSizes = new[] { 20, 20, 30, 30 };
            var columnLabels = new[]
            {
                OutputString.PlanetId, OutputString.TotalMoons, OutputString.PlanetMoonAverageTemperature,
                OutputString.MoonsWithTemperature
            };

            ConsoleWriter.CreateHeader(columnLabels, columnSizes);

            foreach (var planet in planets)
            {
                ConsoleWriter.CreateText(
                    new[]
                    {
                        CultureInfoUtility.TextInfo.ToTitleCase(planet.Id),
                        planet.Moons.Count.ToString(),
                        FormatTemperature(planet.AverageMoonTemperature),
                        planet.MoonsWithTemperature.ToString()
                    },
                    columnSizes);
            }

            ConsoleWriter.CreateLine(columnSizes);
            ConsoleWriter.CreateEmptyLines(2);

            /*
                This is an example of the output (the values are examples):
                --------------------+--------------------+------------------------------+------------------------------
                Planet's Id         |Total Moons         |Average Moon Temperature      |Moons With Temperature
                --------------------+--------------------+------------------------------+------------------------------
                Terre               |1                   |250 K                         |1
                Saturne             |293                 |n/a                           |0
                --------------------+--------------------+------------------------------+------------------------------
            */
        }

        private static string FormatTemperature(float? temperature)
        {
            //The unit is the unit of the API: Kelvin. "n/a" is only shown when the API returned no temperature at all.
            return temperature.HasValue
                ? $"{temperature.Value.ToString("0.##", CultureInfo.CurrentCulture)} {OutputString.TemperatureUnit}"
                : OutputString.NotAvailable;
        }
    }
}
