using AutoMapper;
using BookReviewApp.Models.Domain;
using BookReviewApp.Models.ViewModels;
using BookReviewApp.Models.ViewModels.Api;

namespace BookReviewApp.Common.Mappings
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<Book, BookDto>()
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src =>
                    src.Reviews.Any() ? Math.Round(src.Reviews.Average(r => r.Rating), 2) : 0))
                .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => src.Reviews.Count));

            CreateMap<Book, BookViewModel>()
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src =>
                    src.Reviews.Any() ? Math.Round(src.Reviews.Average(r => r.Rating), 2) : 0))
                .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => src.Reviews.Count));

            CreateMap<CreateBookDto, Book>();
            CreateMap<BookViewModel, Book>();
        }
    }
}