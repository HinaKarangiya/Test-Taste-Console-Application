using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Test_Taste_Console_Application.Domain.DataTransferObjects;

namespace Test_Taste_Console_Application.Domain.Objects
{
    public class Planet
    {
        public string Id { get; set; }
        public float SemiMajorAxis { get; set; }
        public ICollection<Moon> Moons { get; set; }

        public float? AverageMoonTemperature
        {
            get
            {
                var knownTemperatures = KnownMoonTemperatures();
                return knownTemperatures.Count == 0 ? (float?)null : knownTemperatures.Average();
            }
        }

        public int MoonsWithTemperature => KnownMoonTemperatures().Count;

        public Planet(PlanetDto planetDto)
        {
            Id = planetDto.Id;
            SemiMajorAxis = planetDto.SemiMajorAxis;
            Moons = new Collection<Moon>();
            if(planetDto.Moons != null)
            {
                foreach (MoonDto moonDto in planetDto.Moons)
                {
                    Moons.Add(new Moon(moonDto));
                }
            }
        }

        public Boolean HasMoons()
        {
            return (Moons != null && Moons.Count > 0);
        }

        //Moons having temp. value
        private List<float> KnownMoonTemperatures()
        {
            if (!HasMoons()) return new List<float>();

            return Moons
                .Where(moon => moon.AverageTemperature.HasValue)
                .Select(moon => moon.AverageTemperature.Value)
                .ToList();
        }
    }
}
