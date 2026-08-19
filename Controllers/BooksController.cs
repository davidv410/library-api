using LibraryApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryApi.DTOs.Books;
using LibraryApi.Models;
using LibraryApi.Mappers;
using LibraryApi.Services;

namespace LibraryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly BookService _bookService;

    public BooksController(BookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    public async Task<IActionResult> GetBooks()
    {
        var books = await _bookService.GetBooks();

        return Ok(books);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBook(int id)
    {
        var book = await _bookService.GetBook(id);

        if(book == null)
        {
            return NotFound();
        }

        return Ok(book);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateBook(CreateBookDto dto)
    {
        var book = await _bookService.CreateBook(dto);

        return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateBook(int id, UpdateBookDto dto)
    {
        var book = await _bookService.UpdateBook(id, dto);

        if(book == null)
        {
            return NotFound();
        }

        return Ok(book);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var deleted = await _bookService.DeleteBook(id);

        if(!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}