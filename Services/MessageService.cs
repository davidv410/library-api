using LibraryApi.Data;
using LibraryApi.DTOs.Messages;
using LibraryApi.Exceptions;
using LibraryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Services;

public class MessageService : IMessageService
{
    private readonly AppDbContext _db;

    public MessageService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<MessageResponseDto?> SendMessage(string senderId, CreateMessageDto dto)
    {
        if(string.IsNullOrWhiteSpace(senderId))
        {
            return null;
        }

        if(senderId == dto.ReceiverId)
        {
            return null;
        }

        var receiverExists = await _db.Users.AnyAsync(u => u.Id == dto.ReceiverId);
        if (!receiverExists)
        {
            return null;
        }


        var message = new Message
        {
          SenderId = senderId,
          MessageContent = dto.MessageContent,
          ReceiverId = dto.ReceiverId,
          SentAt = DateTime.UtcNow
        };

        _db.Messages.Add(message);

        await _db.SaveChangesAsync();

        return new MessageResponseDto
        {
          Id = message.Id,
          MessageContent = message.MessageContent,
          SentAt = message.SentAt,
          SenderId = message.SenderId,
          ReceiverId = message.ReceiverId
        };
    }

    public async Task<IEnumerable<MessageResponseDto>?> GetConversation(string userId, string receiverId)
    {
        var receiverExists = await _db.Users.AnyAsync(u => u.Id == receiverId);
        if (!receiverExists)
        {
            return null;
        }
        throw new NotImplementedException();
        //check db for messages between users

        //return new MessageResponseDto
    }
}