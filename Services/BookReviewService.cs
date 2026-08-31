using LibraryApi.Data;
using LibraryApi.DTOs.BookReviews;
using LibraryApi.Models;

namespace LibraryApi.Services;

public class BookReviewService : IBookReviewService
{
    private readonly AppDbContext _db;

    public BookReviewService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<BookReviewResponseDto?> CreateBookReview(int bookId, string userId, CreateBookReviewDto dto)
    {
        if(string.IsNullOrWhiteSpace(userId))
        {
            return null;
        }

        var bookExists = await _db.Books.FindAsync(bookId);
        if(bookExists == null)
        {
            return null;
        }

        var bookReview = new BookReview
        {
            Review = dto.Review,
            Rating = dto.Rating,
            BookId = bookId,
            UserId = userId
        };

        _db.BookReviews.Add(bookReview);

        await _db.SaveChangesAsync();

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