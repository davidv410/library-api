using LibraryApi.DTOs.Books;

namespace LibraryApi.Services;

public interface IBookService
{
    Task<IEnumerable<BookResponseDto>> GetBooks();

    Task<BookResponseDto?> GetBook(int id);

    Task<BookResponseDto> CreateBook(CreateBookDto dto);

    Task<BookResponseDto?> UpdateBook(int id, UpdateBookDto dto);

    Task<bool> DeleteBook(int id);
}