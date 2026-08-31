using LibraryApi.Services;
using Microsoft.AspNetCore.Mvc;
using LibraryApi.DTOs.BookReviews;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace LibraryApi.Controllers;

[ApiController]
[Route("api/books/{bookId}/reviews")]

public class BookReviewController : ControllerBase
{
    private readonly IBookReviewService _bookReviewService;

    public BookReviewController(IBookReviewService bookReviewService)
    {
        _bookReviewService = bookReviewService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateBookReview(int bookId, CreateBookReviewDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var review = await _bookReviewService.CreateBookReview(bookId, userId, dto);
        return Ok(review);
    }
}