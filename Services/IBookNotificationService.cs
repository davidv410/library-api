using LibraryApi.DTOs.Books;

namespace LibraryApi.Services;

public interface IBookNotificationService
{
    Task AnnounceNewBook(BookResponseDto book);
}