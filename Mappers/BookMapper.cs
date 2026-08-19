using LibraryApi.DTOs.Books;
using LibraryApi.Models;

namespace LibraryApi.Mappers;

public static class BookMapper
{
    public static BookResponseDto ToResponseDto(Book book)
    {
        return new BookResponseDto
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            ReleaseYear = book.ReleaseYear  
        };
    }
}