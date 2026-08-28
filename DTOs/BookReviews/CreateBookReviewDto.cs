using System.ComponentModel.DataAnnotations;

namespace LibraryApi.DTOs.BookReviews;

public class CreateBookReviewDto
{
    [Required]
    [StringLength(500, MinimumLength = 1)]
    public string Review { get; set; } = String.Empty;

    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }
}