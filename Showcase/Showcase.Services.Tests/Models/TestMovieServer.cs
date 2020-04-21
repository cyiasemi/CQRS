using Showcase.Domain.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Showcase.Services.Tests
{
    public class TestMovieServer : IMovieServer
    {
        /*
            b06da2ab-56df-4072-90ee-1cff9dd181c3
            2774c4f3-0bc2-454d-9e2f-d3d41973ad80
            631329bb-4af0-4c16-af23-7c183361b97a
            1cc02201-e6c3-4a2f-9c8e-6c74e84012f7
            47888df9-f01e-488f-bd09-5a0ad3950da9

         */
        public Task<IEnumerable<Movie>> GetMovies(IDataConverter<List<Movie>> dataConverter)
        {
            var newData = new List<Movie>
            {
                new Movie() { id = Guid.Parse("b06da2ab-56df-4072-90ee-1cff9dd181c3"), headline = "Test Movie 1 Updated", lastUpdated = DateTime.Today.AddDays(11).ToShortDateString(), cardImages = new Cardimage[1] { new Cardimage() { url = "https://www.xyz.com/wp-content/themes/xyz/careers/img/office2.jpg", h = 500, w = 500 } } },
                new Movie() { id = Guid.Parse("2774c4f3-0bc2-454d-9e2f-d3d41973ad80"), headline = "Test Movie 2", lastUpdated = DateTime.Today.AddDays(1).ToShortDateString() },
                new Movie() { id = Guid.Parse("631329bb-4af0-4c16-af23-7c183361b97a"), headline = "Test Movie 3.", lastUpdated = DateTime.Today.AddDays(1).ToShortDateString() },
                new Movie() { id = Guid.Parse("1cc02201-e6c3-4a2f-9c8e-6c74e84012f7"), headline = "Test Movie 4.", lastUpdated = DateTime.Today.AddDays(1).ToShortDateString() },
                new Movie() { id = Guid.Parse("47888df9-f01e-488f-bd09-5a0ad3950da9"), headline = "Test Movie 5.", lastUpdated = DateTime.Today.AddDays(1).ToShortDateString() }
            };
            return Task.FromResult((IEnumerable<Movie>)newData);
        }
    }

}
