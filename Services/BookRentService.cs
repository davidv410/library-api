using LibraryApi.Data;

namespace LibraryApi.Services;

public class BookRentService : IBookRentService
{
    private readonly AppDbContext _db;

    public BookRentService(AppDbContext db)
    {
        _db = db;
    }
    public async Task RentBook(int bookId, string userId)
    {
        //check userId
        //start transaction
        //find book + lock
        //if !book fail
        //check if already rented
        //check available copies
        //insert
        //update available copies
        //save changes
        //return response
        //end transaction
    }
}