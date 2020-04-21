using Showcase.Domain.Core.Dtos;
using Showcase.Service.Application.Mappers;
using System;
using Xunit;

namespace Showcase.Services.MappingTests
{
    public class MappingTests
    {
        [Theory]
        [InlineData(typeof(Movie), typeof(MovieDto))]
        public void ShouldBeMappedToMovieDto(Type source, Type destination)
        {
            var instance = Activator.CreateInstance(source);
            var mapper = new MovieMapper();
            var mapped = mapper.MapMovieDto(instance as Movie);
            Assert.Equal(mapped.GetType(), destination);
        }

        [Theory]
        [InlineData(typeof(Movie), typeof(MovieBaseDto))]
        public void ShouldBeMappedToMovieBaseDto(Type source, Type destination)
        {
            var instance = Activator.CreateInstance(source);
            var mapper = new MovieMapper();
            var mapped = mapper.MapMovieBaseDto(instance as Movie);
            Assert.Equal(mapped.GetType(), destination);
        }
    }
}
