using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Test_Taste_Console_Application.Constants;
using Test_Taste_Console_Application.Domain.DataTransferObjects;
using Test_Taste_Console_Application.Domain.Objects;
using Test_Taste_Console_Application.Domain.Services.Interfaces;
using Test_Taste_Console_Application.Utilities;

namespace Test_Taste_Console_Application.Domain.Services
{
    /// <inheritdoc />
    public class PlanetService : IPlanetService
    {
        private readonly ISolarSystemService _solarSystemService;

        //The planets are needed by more than one output, so they are only loaded once.
        private Task<IReadOnlyList<Planet>> _planets;

        public PlanetService(ISolarSystemService solarSystemService)
        {
            _solarSystemService = solarSystemService;
        }

        public async Task<IReadOnlyList<Planet>> GetAllPlanetsAsync()
        {
            try
            {
                return await (_planets ??= LoadPlanetsAsync());
            }
            catch
            {
                //A failed load isn't kept, so that the next caller can try again.
                _planets = null;
                throw;
            }
        }

        private async Task<IReadOnlyList<Planet>> LoadPlanetsAsync()
        {
            //Load all planets
            var planetDtos = await _solarSystemService.GetPlanetsAsync();

            var moonIds = planetDtos
                        .SelectMany(planetDto => planetDto.Moons ?? Enumerable.Empty<MoonDto>())
                        .Select(moonReference => moonReference.URLId)
                        .Where(moonId => !string.IsNullOrEmpty(moonId))
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToArray();

            //Load all all moons details
            var moonDetailsById = await LoadMoonDetailsAsync(moonIds);

            //Prepare & return planets
            var allPlanets = new List<Planet>();
            foreach (var planetDto in planetDtos)
            {
                var moonsOfPlanet = new List<MoonDto>();
                foreach (var moonReference in planetDto.Moons ?? Enumerable.Empty<MoonDto>())
                {
                    var moonId = moonReference.URLId;
                    moonsOfPlanet.Add(moonId != null && moonDetailsById.TryGetValue(moonId, out var moonDetails)
                                        ? moonDetails
                                        : moonReference);
                }

                allPlanets.Add(new Planet(new PlanetDto
                {
                    Id = planetDto.Id,
                    SemiMajorAxis = planetDto.SemiMajorAxis,
                    Moons = moonsOfPlanet
                }));
            }

            return allPlanets;
        }

        /// Loads the details of all the moons with GET /bodies/{id}. A few requests run at the same time.
        private async Task<Dictionary<string, MoonDto>> LoadMoonDetailsAsync(string[] moonIds)
        {
            var moonDetailsById = new Dictionary<string, MoonDto>(StringComparer.OrdinalIgnoreCase);
            if (moonIds.Length == 0) return moonDetailsById;

            Logger.Instance.Info(string.Format(LoggerMessage.MoonDetailsLoading, moonIds.Length));

            //The moons are requested in groups. A group is finished before the next one starts, so a failure never leaves requests running in the background.
            foreach (var group in moonIds.Chunk(HttpClientSettings.MaxParallelRequests))
            {
                var moonDetails = await Task.WhenAll(group.Select(LoadMoonDetailsOrNullAsync));
                for (var i = 0; i < group.Length; i++)
                {
                    if (moonDetails[i] != null) moonDetailsById[group[i]] = moonDetails[i];
                }
            }

            Logger.Instance.Info(string.Format(LoggerMessage.MoonDetailsLoaded, moonDetailsById.Count));
            return moonDetailsById;
        }

        /// Loads the details of one moon. A moon that can't be found (or has an invalid response) doesn't stop the whole output:
        private async Task<MoonDto> LoadMoonDetailsOrNullAsync(string moonId)
        {
            try
            {
                return await _solarSystemService.GetMoonByIdAsync(moonId);
            }
            catch (HttpRequestException exception) when (!IsProblemForEveryRequest(exception))
            {
                Logger.Instance.Warn($"{string.Format(LoggerMessage.MoonDetailsMissing, moonId)} {exception.Message}");
                return null;
            }
        }

        private static bool IsProblemForEveryRequest(HttpRequestException exception)
        {
            return exception.StatusCode == HttpStatusCode.Unauthorized
                   || exception.StatusCode == HttpStatusCode.Forbidden
                   || exception.StatusCode == (HttpStatusCode)429;
        }
    }
}
