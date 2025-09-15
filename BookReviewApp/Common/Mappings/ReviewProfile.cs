using AutoMapper;
using BookReviewApp.Models.Domain;
using BookReviewApp.Models.ViewModels;
using BookReviewApp.Models.ViewModels.Api;

namespace BookReviewApp.Common.Mappings
{
    public class ReviewProfile : Profile
    {
        public ReviewProfile()
        {
            CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName ?? "Unknown"))
                .ForMember(dest => dest.UpvoteCount, opt => opt.MapFrom(src => src.ReviewVotes.Count(rv => rv.IsUpvote)))
                .ForMember(dest => dest.DownvoteCount, opt => opt.MapFrom(src => src.ReviewVotes.Count(rv => !rv.IsUpvote)))
                .ForMember(dest => dest.NetVotes, opt => opt.MapFrom(src =>
                    src.ReviewVotes.Count(rv => rv.IsUpvote) - src.ReviewVotes.Count(rv => !rv.IsUpvote)));

            CreateMap<Review, ReviewViewModel>()
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName ?? "Unknown"))
                .ForMember(dest => dest.UpvoteCount, opt => opt.MapFrom(src => src.ReviewVotes.Count(rv => rv.IsUpvote)))
                .ForMember(dest => dest.DownvoteCount, opt => opt.MapFrom(src => src.ReviewVotes.Count(rv => !rv.IsUpvote)));

            CreateMap<CreateReviewDto, Review>();
            CreateMap<ReviewViewModel, Review>();
        }
    }
}