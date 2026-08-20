using System.ComponentModel.DataAnnotations;

namespace LibraryApi.DTOs.Books;

public class CreateBookDto
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Author { get; set; } = string.Empty;

    [Range(0, 2026)]
    public int ReleaseYear { get; set; }
}