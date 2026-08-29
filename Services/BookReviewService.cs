using LibraryApi.Data;
using LibraryApi.DTOs.BookReviews;

namespace LibraryApi.Services;

public class BookReviewService : IBookReviewService
{
    private readonly AppDbContext _db;

    public BookReviewService(AppDbContext db)
    {
        _db = db;
    }

    public Task<BookReviewResponseDto> CreateBookReview(int bookId, string userId, CreateBookReviewDto dto)
    {
    }
}