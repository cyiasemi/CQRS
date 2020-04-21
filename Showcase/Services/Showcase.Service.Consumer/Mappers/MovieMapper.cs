using AutoMapper;
using Showcase.Domain.Core.Dtos;
using Showcase.Domain.Core.Interfaces;

namespace Showcase.Service.Application.Mappers
{
    public class MovieMapper : IMovieMapper
    {
        private readonly IMapper _mapper;

        public MovieMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Movie, MovieDto>();
                cfg.CreateMap<Movie, MovieBaseDto>();
            });

            _mapper = config.CreateMapper();
        }

        public MovieDto MapMovieDto(Movie movie)
        {
            return _mapper.Map<Movie, MovieDto>(movie);
        }
        public MovieBaseDto MapMovieBaseDto(Movie movie)
        {
            return _mapper.Map<Movie, MovieBaseDto>(movie);
        }
    }
}
