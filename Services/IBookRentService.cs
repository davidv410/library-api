using LibraryApi.DTOs.BookRent;

namespace LibraryApi.Services;

public interface IBookRentService
{
    Task<BookRentResponseDto> RentBook(int bookId, string userId);
}