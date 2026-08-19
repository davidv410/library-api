namespace LibraryApi.DTOs.Books;

public class UpdateBookDto
{
    public string? Title { get; set; }
    public string? Author { get; set; }
    public int? ReleaseYear { get; set; }
}