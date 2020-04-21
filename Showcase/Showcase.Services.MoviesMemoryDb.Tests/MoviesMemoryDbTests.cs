using Showcase.Services.MemoryDb.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Showcase.Services.MoviesMemoryDb.Tests
{
    public class MoviesMemoryDbTests
    {


        [Theory]
        [ClassData(typeof(TestDataGenerator))]
        public async Task ShouldCreateNewMovie(Guid movieId)
        {
            var repo = new MoviesMemoryRepository(TimeSpan.FromSeconds(10));
            await repo.Add(new Movie() { id = movieId });

            Assert.True(await repo.Any(s => s.id == movieId));

            //clean up
            await repo.RemoveAll(new List<Guid>() { movieId });
        }
        [Theory]
        [ClassData(typeof(TestDataGenerator))]
        public async Task ShouldCreateAnewMovieAndDeleteIt(Guid movieId)
        {
            var repo = new MoviesMemoryRepository(TimeSpan.FromSeconds(10));
            await repo.Add(new Movie() { id = movieId });
            await repo.RemoveAll(new List<Guid>() { movieId });

            Assert.False(await repo.Any(s => s.id == movieId));
        }

        [Theory]
        [ClassData(typeof(TestDataGenerator))]
        public async Task ShouldCreateANewMovieAndUpdateIt(Guid movieId)
        {
            var original = "Unchanged";
            var afterUpdate = "Changed";
            var repo = new MoviesMemoryRepository(TimeSpan.FromSeconds(10));
            await repo.Add(new Movie() { body = original, id = movieId });
            await repo.Update(new Movie() { body = afterUpdate, id = movieId });



            Assert.False(await repo.Any(s => s.id == movieId && s.body == original));
            Assert.True(await repo.Any(s => s.id == movieId && s.body == afterUpdate));

            //clean up
            await repo.RemoveAll(new List<Guid>() { movieId });
        }
        [Fact]
        public async Task ShouldBeReturningTheEvaluationOfAGivenExpression()
        {
            var body1 = "body1";
            var body2 = "body2";
            var body3 = "body3";
            var body4 = "body4";
            var repo = new MoviesMemoryRepository(TimeSpan.FromSeconds(10));
            var movieId1 = Guid.NewGuid();
            var movieId2 = Guid.NewGuid();
            var movieId3 = Guid.NewGuid();
            var movieId4 = Guid.NewGuid();

            await repo.Add(new Movie() { body = body1, id = movieId1 });
            await repo.Add(new Movie() { body = body2, id = movieId2 });
            await repo.Add(new Movie() { body = body3, id = movieId3 });
            await repo.Add(new Movie() { body = body4, id = movieId4 });

            var movie = await repo.Select(s => s.id == movieId2);


            Assert.Equal(movie.First().body, body2);

            //clean up
            await repo.RemoveAll(new List<Guid>() { movieId1, movieId2, movieId3, movieId4 });
        }
        [Fact]
        public async Task ShouldReturnOnlyOneMovieGivenAnId()
        {
            var body1 = "body1";
            var body2 = "body2";
            var body3 = "body3";
            var body4 = "body4";
            var repo = new MoviesMemoryRepository(TimeSpan.FromSeconds(10));
            var movieId1 = Guid.NewGuid();
            var movieId2 = Guid.NewGuid();
            var movieId3 = Guid.NewGuid();
            var movieId4 = Guid.NewGuid();

            await repo.Add(new Movie() { body = body1, id = movieId1 });
            await repo.Add(new Movie() { body = body2, id = movieId2 });
            await repo.Add(new Movie() { body = body3, id = movieId3 });
            await repo.Add(new Movie() { body = body4, id = movieId4 });

            var movie = await repo.Select(s => s.id == movieId2);


            Assert.True(movie.Count() == 1);

            //clean up
            await repo.RemoveAll(new List<Guid>() { movieId1, movieId2, movieId3, movieId4 });
        }

        [Fact]
        public async Task ShouldReturnTwoMoviesGivenAnExpressionThatFitsTwoMovies()
        {
            var body1 = "body1";
            var body2 = "body2";
            var body3 = "body2";
            var body4 = "body4";
            var repo = new MoviesMemoryRepository(TimeSpan.FromSeconds(10));
            var movieId1 = Guid.NewGuid();
            var movieId2 = Guid.NewGuid();
            var movieId3 = Guid.NewGuid();
            var movieId4 = Guid.NewGuid();

            await repo.Add(new Movie() { body = body1, id = movieId1 });
            await repo.Add(new Movie() { body = body2, id = movieId2 });
            await repo.Add(new Movie() { body = body3, id = movieId3 });
            await repo.Add(new Movie() { body = body4, id = movieId4 });

            var movie = await repo.Select(s => s.body == body3);


            Assert.True(movie.Count() == 2);

            //clean up
            await repo.RemoveAll(new List<Guid>() { movieId1, movieId2, movieId3, movieId4 });
        }
    }

}
