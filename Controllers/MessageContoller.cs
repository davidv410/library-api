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

    public MessageController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> SendMessage(CreateMessageDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var saveMessage = await _messageService.SendMessage(userId, dto);
        if(saveMessage == null)
        {
            return BadRequest("Invalid message");
        }
        return Ok(saveMessage);
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