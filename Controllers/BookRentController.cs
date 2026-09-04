using System.Security.Claims;
using LibraryApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers;

[ApiController]
[Route("api/books/{bookId}/rent")]
public class BookRentController : ControllerBase
{
    private readonly IBookRentService _bookRentService;

    public BookRentController(IBookRentService bookRentService)
    {
        _bookRentService = bookRentService;
    }
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> RentBook (int bookId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var rent = _bookRentService.RentBook(bookId, userId);
        return Ok(rent);
    }
}