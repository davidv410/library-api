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

        return Ok(books);
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

        return Ok(book);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBook(int id)
    {
        var book = await _db.Books.FindAsync(id);

        if(book == null)
        {
            return NotFound();
        }

        return Ok(book);
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
}