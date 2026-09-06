using LibraryApi.Data;
using LibraryApi.Exceptions;
using LibraryApi.Models;
using LibraryApi.DTOs.BookRent;
using LibraryApi.Migrations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Services;

public class BookRentService : IBookRentService
{
    private readonly AppDbContext _db;

    public BookRentService(AppDbContext db)
    {
        _db = db;
    }
    public async Task<BookRentResponseDto> RentBook(int bookId, string userId)
    {
        if(string.IsNullOrWhiteSpace(userId))
        {
            throw new AppException(StatusCodes.Status401Unauthorized, "User not logged in");
        }

        using var transaction = await _db.Database.BeginTransactionAsync();

        var book = await _db.Books
            .FromSqlInterpolated($"SELECT * FROM \"Books\" WHERE \"Id\" = {bookId} FOR UPDATE")
            .FirstOrDefaultAsync(b => b.Id == bookId);
        if(book == null)
        {
            throw new AppException(StatusCodes.Status404NotFound, "Book not found");
        }

        var rented = await _db.RentedBooks.AnyAsync(r => r.BookId == bookId && r.UserId == userId);
        if(rented)
        {
            throw new AppException(StatusCodes.Status409Conflict, "Book already rented");
        }

        if(book.AvailableCopies == 0)
        {
            throw new AppException(StatusCodes.Status409Conflict, "No available copies");
        }

        var rentedBook = new RentedBook
        {
            BookId = bookId,
            UserId = userId,
            RentedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(14),
        };

        book.AvailableCopies --;

        _db.RentedBooks.Add(rentedBook);
        await _db.SaveChangesAsync();
        await transaction.CommitAsync();

        return new BookRentResponseDto
        {
            Id = rentedBook.Id,
            BookId = rentedBook.BookId,
            RentedAt = rentedBook.RentedAt,
            DueDate = rentedBook.DueDate,
            ReturnedAt = rentedBook.ReturnedAt,
            UserId = rentedBook.UserId
        };
    }
}