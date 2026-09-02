using LibraryApi.Hubs;
using Microsoft.AspNetCore.SignalR;
using LibraryApi.DTOs.Messages;
using LibraryApi.Services;

public class MessageNotificationService : IMessageNotificationService
{
    private readonly IHubContext<MessagingHub> _hubContext;

    public MessageNotificationService (IHubContext<MessagingHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NewMessage (MessageResponseDto message)
    {
        await _hubContext.Clients.User(message.ReceiverId).SendAsync("PrivateMessage", message);
    }
}