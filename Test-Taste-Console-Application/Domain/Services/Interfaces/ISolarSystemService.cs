using System.Collections.Generic;
using System.Threading.Tasks;
using Test_Taste_Console_Application.Domain.DataTransferObjects;

namespace Test_Taste_Console_Application.Domain.Services.Interfaces
{
    ///<summary>
    /// The service that talks to the Solar System OpenData API<see href="https://api.le-systeme-solaire.net/"/ & being used for moon & planet services as well>:
    /// the only place that knows about HTTP, the URLs, paging and JSON of the API.
    ///</summary>
    public interface ISolarSystemService
    {
        Task<IReadOnlyList<PlanetDto>> GetPlanetsAsync();

        Task<IReadOnlyList<MoonDto>> GetMoonsWithMassAsync();

        Task<MoonDto> GetMoonByIdAsync(string id);
    }
}
