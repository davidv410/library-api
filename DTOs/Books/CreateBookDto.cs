using System.ComponentModel.DataAnnotations;

namespace LibraryApi.DTOs.Books;

public class CreateBookDto
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Author { get; set; } = string.Empty;

    [Range(0, 2026)]
    public int ReleaseYear { get; set; }
}