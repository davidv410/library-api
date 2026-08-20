using System.ComponentModel.DataAnnotations;

namespace LibraryApi.DTOs.Books;

public class UpdateBookDto
{
    [StringLength(200, MinimumLength = 1)]
    public string? Title { get; set; }

    [StringLength(100, MinimumLength = 1)]
    public string? Author { get; set; }

    [Range(0, 2026)]
    public int? ReleaseYear { get; set; }
}