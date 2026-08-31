using LibraryApi.Data;
using LibraryApi.DTOs.BookReviews;
using LibraryApi.Mappers;
using LibraryApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using LibraryApi.Exceptions;

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
            throw new AppException(StatusCodes.Status404NotFound, "Book was not found");
        }

        var bookReviews = await _db.BookReviews.Where(book => book.BookId == bookId).ToListAsync();

        return bookReviews.Select(BookReviewMapper.ToResponseDto);
    }

    public async Task<BookReviewResponseDto?> CreateBookReview(int bookId, string userId, CreateBookReviewDto dto)
    {
        if(string.IsNullOrWhiteSpace(userId))
        {
            throw new AppException(StatusCodes.Status401Unauthorized, "User not logged in");
        }

        var bookExists = await _db.Books.FindAsync(bookId);
        if(bookExists == null)
        {
            throw new AppException(StatusCodes.Status404NotFound, "Book was not found");
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

        return BookReviewMapper.ToResponseDto(bookReview);
    }
}