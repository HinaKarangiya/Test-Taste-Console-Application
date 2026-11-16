using System.Collections.Generic;
using System.Threading.Tasks;
using Test_Taste_Console_Application.Domain.Objects;

namespace Test_Taste_Console_Application.Domain.Services.Interfaces
{
    ///<summary>
    /// A service that can be used to get planets (with their moons) from the Solar System OpenData API<see href="https://api.le-systeme-solaire.net/"/>.
    ///</summary>
    public interface IPlanetService
    {
        Task<IReadOnlyList<Planet>> GetAllPlanetsAsync();
    }
}
