using System.ComponentModel.DataAnnotations;

namespace LibraryApi.DTOs.Books;

public class UpdateBookDto
{
    [MinLength(1)]
    public string? Title { get; set; }

    [MinLength(1)]
    public string? Author { get; set; }

    [Range(0, 2026)]
    public int? ReleaseYear { get; set; }
}