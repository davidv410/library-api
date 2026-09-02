using LibraryApi.DTOs.Messages;

namespace LibraryApi.Services;
public interface IMessageNotificationService
{
    Task NewMessage (MessageResponseDto dto);
}