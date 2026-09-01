using LibraryApi.DTOs.Books;
using LibraryApi.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace LibraryApi.Services;

public class BookNotificationService : IBookNotificationService
{
    private readonly IHubContext<MessagingHub> _hubContext;

    public BookNotificationService (IHubContext<MessagingHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task AnnounceNewBook(BookResponseDto book)
    {
        await _hubContext.Clients.All.SendAsync("NewBookAnnounced", book);
    }
}