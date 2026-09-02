using LibraryApi.DTOs.Messages;

namespace LibraryApi.Services;

public interface IMessageService
{
    Task<MessageResponseDto?> SendMessage(string senderId, CreateMessageDto dto);
    Task<IEnumerable<MessageResponseDto>?> GetConversation(string userId, string receiverId);
}