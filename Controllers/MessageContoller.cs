using LibraryApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LibraryApi.DTOs.Messages;
using System.Security.Claims;

namespace LibraryApi.Controllers;

[ApiController]
[Route("api/messages")]
public class MessageController : ControllerBase
{
    private readonly IMessageService _messageService;
    private readonly IMessageNotificationService _messageNotificationService;

    public MessageController(IMessageService messageService, IMessageNotificationService messageNotificationService)
    {
        _messageService = messageService;
        _messageNotificationService = messageNotificationService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> SendMessage(CreateMessageDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var message = await _messageService.SendMessage(userId, dto);
        if(message == null)
        {
            return BadRequest("Invalid message");
        }

        await _messageNotificationService.NewMessage(message);
        return Ok(message);
    }

    [HttpGet("{receiverId}")]
    [Authorize]
    public async Task<IActionResult> GetConversation(string receiverId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var conversation = await _messageService.GetConversation(userId, receiverId);
        return Ok(conversation);
    }
}