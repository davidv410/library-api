using LibraryApi.Data;
using LibraryApi.DTOs.Books;
using LibraryApi.Mappers;
using LibraryApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace LibraryApi.Services;

public class BookService
{
    private readonly AppDbContext _db;

    public BookService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<BookResponseDto>> GetBooks()
    {
        var books = await _db.Books.ToListAsync();

        return books.Select(BookMapper.ToResponseDto);
    }

    public async Task<BookResponseDto?> GetBook(int id)
    {
        var book = await _db.Books.FindAsync(id);

        if(book == null)
        {
            return null;
        }

        return BookMapper.ToResponseDto(book);
    }


    public async Task<BookResponseDto> CreateBook(CreateBookDto dto)
    {
        var book = new Book
        {
          Title = dto.Title,
          Author = dto.Author,
          ReleaseYear = dto.ReleaseYear,  
        };

        _db.Books.Add(book);

        await _db.SaveChangesAsync();

        return BookMapper.ToResponseDto(book);
    }

    public async Task<BookResponseDto?> UpdateBook(int id, UpdateBookDto dto)
    {
        var book = await _db.Books.FindAsync(id);

        if(book == null)
        {
            return null;
        }

        if(dto.Title != null)
        {
            book.Title = dto.Title;
        }

        if(dto.Author != null)
        {
            book.Author = dto.Author;
        }

        if (dto.ReleaseYear.HasValue)
        {
            book.ReleaseYear = dto.ReleaseYear.Value;
        }

        await _db.SaveChangesAsync();

        return BookMapper.ToResponseDto(book);
    }

    public async Task<bool> DeleteBook(int id)
    {
        var book = await _db.Books.FindAsync(id);

        if(book == null)
        {
            return false;
        }

        _db.Books.Remove(book);

        await _db.SaveChangesAsync();

        return true;
    }
}