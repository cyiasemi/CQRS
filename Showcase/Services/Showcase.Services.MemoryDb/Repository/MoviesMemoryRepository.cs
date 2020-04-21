using Showcase.Domain.Core.Interfaces;
using Showcase.Services.MemoryDb.Context;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Showcase.Services.MemoryDb.Repository
{
    public class MoviesMemoryRepository : IMovieRepository, ICacheMovieRepository
    {
        private readonly MovieDbContext _db = new MovieDbContext();
        public TimeSpan CachingTime { get; }
        public MoviesMemoryRepository(TimeSpan moviesCachingTime)
        {
            CachingTime = moviesCachingTime;
        }
        public async Task<int> Count()
        {
            return await Task.FromResult(_db.Movies.Count);
        }
        public Task<bool> Any(Predicate<Movie> contition)
        {
            foreach (var movie in _db.Movies.Values)
            {
                if (contition(movie))
                {
                    return Task.FromResult(true);
                }
            }
            return Task.FromResult(false);
        }
        public Task<IEnumerable<Movie>> Select(Predicate<Movie> contition)
        {
            var results = new List<Movie>();
            foreach (var movie in _db.Movies.Values)
            {
                if (contition(movie))
                {
                    results.Add(movie);
                }
            }
            return Task.FromResult((IEnumerable<Movie>)results);

        }
        public Task<DateTimeOffset> GetLastUpdate()
        {
            return Task.FromResult(_db.LastUpdated);
        }
        public async Task<Movie> Add(Movie movie)
        {
            _db.Movies.Add(movie.id, movie);
            await InformLastUpdate();

            return _db.Movies[movie.id];
        }
        public async Task AddRange(IEnumerable<Movie> movies)
        {
            foreach (var movie in movies)
                _db.Movies.Add(movie.id, movie);
            await InformLastUpdate();
        }
        public async Task<bool> RemoveAll(IEnumerable<Guid> ids)
        {
            foreach (var id in ids)
            {
                _db.Movies.Remove(id);
            }
            await InformLastUpdate();

            return true;
        }
        public async Task<bool> SetLastUpdate()
        {
            await InformLastUpdate();
            return true;
        }
        public async Task AddOrUpdate(Movie movie)
        {
            if (_db.Movies.ContainsKey(movie.id))
                _db.Movies[movie.id] = movie;
            else
                _db.Movies.Add(movie.id, movie);

            await InformLastUpdate();
        }
        public async Task<Movie> Update(Movie movie)
        {
            _db.Movies[movie.id] = movie;
            await InformLastUpdate();

            return _db.Movies[movie.id];
        }
        private Task<bool> InformLastUpdate()
        {
            _db.LastUpdated = DateTimeOffset.UtcNow;
            return Task.FromResult(true);
        }
    }
}
