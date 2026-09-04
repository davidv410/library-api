namespace LibraryApi.Services;

public interface IBookRentService
{
    Task RentBook(int bookId, string userId);
}