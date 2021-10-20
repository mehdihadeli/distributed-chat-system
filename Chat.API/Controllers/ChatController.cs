using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chat.API.Controllers.ViewModels;
using Chat.Application.DTOs;
using Chat.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;

namespace Chat.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        HubConnection connection;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
            connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:5001/chatSignalr")
                .Build();
        }

        [HttpPost]
        [Route("send-message")]
        public async Task<ActionResult> SendMessageAsync(SendMessageRequest request)
        {
            var sendMessageDto = new SendMessageDto
            {
                Message = request.Message,
                SenderUserName = request.SenderUserName,
                TargetUserName = request.TargetUserName
            };
            await _chatService.SendMessageAsync(sendMessageDto);

            // directly send to signalr hub instead of broker
            // await connection.StartAsync();
            // await connection.InvokeAsync("SendMessage",sendMessageDto);

            return NoContent();
        }

        [HttpGet("load-messages/{userName}")]
        public async Task<ActionResult<IEnumerable<ChatMessageDto>>> LoadMessages([FromRoute] string userName,
            [FromQuery] int numberOfMessages = 50)
        {
            var messages = await _chatService.LoadMessagesByCount(userName, numberOfMessages);

            return Ok(messages);
        }

        [HttpGet("load-messages-by-time/{userName}")]
        public async Task<ActionResult<IEnumerable<ChatMessageDto>>> LoadMessagesByTime([FromRoute] string userName,
            [FromQuery] DateTime dateTime)
        {
            var messages = await _chatService.LoadMessagesByTime(userName, dateTime);

            return Ok(messages);
        }
    }
}