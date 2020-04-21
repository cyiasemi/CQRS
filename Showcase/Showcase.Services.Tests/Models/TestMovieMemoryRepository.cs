using Showcase.Domain.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Showcase.Services.Tests
{
    public class TestMovieMemoryRepository : IMovieRepository, ICacheMovieRepository
    {
        public TimeSpan CachingTime => TimeSpan.FromMinutes(10);
        private readonly List<Movie> _fakeDatabase = new List<Movie>();
        private readonly bool _isUpdated = true;

        public TestMovieMemoryRepository()
        {
            _fakeDatabase.Add(new Movie() { id = Guid.Parse("b06da2ab-56df-4072-90ee-1cff9dd181c3"), headline = "Test Movie 1", body = "Test Movie 1", lastUpdated = DateTime.Today.AddDays(1).ToShortDateString(), cardImages = new Cardimage[1] { new Cardimage() { url = "https://www.xyz.com/wp-content/themes/xyz/careers/img/office2.jpg", h = 500, w = 500 } } });

        }
        public TestMovieMemoryRepository(bool isUpdated)
        {
            _fakeDatabase.Add(new Movie() { id = Guid.Parse("b06da2ab-56df-4072-90ee-1cff9dd181c3"), headline = "Test Movie 1 Updated", body = "Test Movie 1 Updated", lastUpdated = DateTime.Today.AddDays(-12).ToShortDateString(), cardImages = new Cardimage[1] { new Cardimage() { url = "https://www.xyz.com/wp-content/themes/xyz/careers/img/office2.jpg", h = 500, w = 500 } } });
            _isUpdated = isUpdated;
        }
        public Task<Movie> Add(Movie movie)
        {
            _fakeDatabase.Add(movie);
            return Task.FromResult(movie);
        }

        public Task AddOrUpdate(Movie movie)
        {
            var valueToUpdate = _fakeDatabase.FirstOrDefault(m => m.id == movie.id);
            if (valueToUpdate == null)
                _fakeDatabase.Add(movie);
            else
                _fakeDatabase[_fakeDatabase.IndexOf(valueToUpdate)] = movie;
            return Task.FromResult(1);
        }

        public Task AddRange(IEnumerable<Movie> movies)
        {
            foreach (var movie in movies)
            {
                _fakeDatabase.Add(movie);
            }

            return Task.FromResult(true);
        }

        public Task<bool> Any(Predicate<Movie> contition)
        {
            foreach (var movie in _fakeDatabase)
            {
                if (contition(movie))
                {
                    return Task.FromResult(true);
                }
            }
            return Task.FromResult(false);
        }

        public Task<int> Count()
        {
            throw new NotImplementedException();
        }

        public Task<DateTimeOffset> GetLastUpdate()
        {
            if (_isUpdated == true)
                return Task.FromResult(DateTimeOffset.UtcNow);
            else
            {
                return Task.FromResult(DateTimeOffset.UtcNow.AddDays(-1));
            }
        }

        public Task<bool> RemoveAll(IEnumerable<Guid> ids)
        {
            _fakeDatabase.RemoveAll(s => ids.Contains(s.id));
            return Task.FromResult(true);
        }

        public async Task<IEnumerable<Movie>> Select(Predicate<Movie> contition)
        {
            var results = new List<Movie>();
            foreach (var movie in _fakeDatabase)
            {
                if (contition(movie))
                {
                    results.Add(movie);
                }
            }
            return await Task.FromResult((IEnumerable<Movie>)results);
        }

        public Task<bool> SetLastUpdate()
        {
            return Task.FromResult(true);
        }

        public Task<Movie> Update(Movie movie)
        {
            AddOrUpdate(movie);

            return Task.FromResult(movie);
        }
    }

}
