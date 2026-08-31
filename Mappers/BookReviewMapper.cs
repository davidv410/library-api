using LibraryApi.DTOs.BookReviews;
using LibraryApi.Models;

namespace LibraryApi.Mappers;

public static class BookReviewMapper
{
    public static BookReviewResponseDto ToResponseDto(BookReview bookReview)
    {
        return new BookReviewResponseDto
        {
            Id = bookReview.Id,
            Review = bookReview.Review,
            Rating = bookReview.Rating,
            LikeCount = bookReview.LikeCount,
            DislikeCount = bookReview.DislikeCount,
            BookId = bookReview.BookId,
            UserId = bookReview.UserId
        };
    }
}