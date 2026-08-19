using LibraryApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryApi.DTOs.Books;
using LibraryApi.Models;

namespace LibraryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly AppDbContext _db;

    public BooksController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetBooks()
    {
        var books = await _db.Books.ToListAsync();

        var response = books.Select(book => new BookResponseDto
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            ReleaseYear = book.ReleaseYear
        });

        return Ok(books);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBook(int id)
    {
        var book = await _db.Books.FindAsync(id);

        if(book == null)
        {
            return NotFound();
        }

        return Ok(new BookResponseDto
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            ReleaseYear = book.ReleaseYear
        });
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateBook(CreateBookDto dto)
    {
        var book = new Book
        {
            Title = dto.Title,
            Author = dto.Author,
            ReleaseYear = dto.ReleaseYear
        };

        _db.Books.Add(book);

        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateBook(int id, UpdateBookDto dto)
    {
        var book = await _db.Books.FindAsync(id);

        if(book == null)
        {
            return NotFound();
        }

        if(dto.Title != null)
        {
            book.Title = dto.Title;
        }

        if(dto.Author != null)
        {
            book.Author = dto.Author;
        }

        if(dto.ReleaseYear.HasValue)
        {
            book.ReleaseYear = dto.ReleaseYear.Value;
        }

        await _db.SaveChangesAsync();

        return Ok(book);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var book = await _db.Books.FindAsync(id);

        if(book == null)
        {
            return NotFound();
        }

        _db.Books.Remove(book);

        await _db.SaveChangesAsync();

        return NoContent();
    }
}