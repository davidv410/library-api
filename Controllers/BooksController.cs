using LibraryApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryApi.DTOs.Books;
using LibraryApi.Models;
using LibraryApi.Mappers;
using LibraryApi.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace LibraryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    private readonly IBookNotificationService _bookNotificationService;

    public BooksController(IBookService bookService, IBookNotificationService bookNotificationService)
    {
        _bookService = bookService;
        _bookNotificationService = bookNotificationService;
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
    
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateBook(CreateBookDto dto)
    {
        var book = await _bookService.CreateBook(dto);

        await _bookNotificationService.AnnounceNewBook(book);
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
    }

    [Authorize(Roles = "Admin")]
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

    [Authorize(Roles = "Admin")]
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