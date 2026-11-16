using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Test_Taste_Console_Application.Domain.Objects;
using Test_Taste_Console_Application.Domain.Services.Interfaces;

namespace Test_Taste_Console_Application.Domain.Services
{
    /// <inheritdoc />
    public class MoonService : IMoonService
    {
        private readonly ISolarSystemService _solarSystemService;

        public MoonService(ISolarSystemService solarSystemService)
        {
            _solarSystemService = solarSystemService;
        }

        public async Task<IReadOnlyList<Moon>> GetAllMoonsAsync()
        {
            //The data access layer does the request, the paging and the error handling.
            var moonDtos = await _solarSystemService.GetMoonsWithMassAsync();

            //The data transfer objects are turned into domain objects.
            return moonDtos.Select(moonDto => new Moon(moonDto)).ToList();
        }
    }
}
