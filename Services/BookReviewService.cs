using LibraryApi.Data;
using LibraryApi.DTOs.BookReviews;
using LibraryApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Services;

public class BookReviewService : IBookReviewService
{
    private readonly AppDbContext _db;

    public BookReviewService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<BookReviewResponseDto>?> GetBookReviews(int bookId)
    {
        var bookExists = await _db.Books.FindAsync(bookId);
        if(bookExists == null)
        {
            return null;
        }

        var bookReviews = await _db.BookReviews.Where(book => book.BookId == bookId).Select(book => new BookReviewResponseDto
        {
            Id = book.Id,
            Review = book.Review,
            Rating = book.Rating,
            LikeCount = book.LikeCount,
            DislikeCount = book.DislikeCount,
            BookId = book.BookId,
            UserId = book.UserId
        }).ToListAsync();

        return bookReviews;
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