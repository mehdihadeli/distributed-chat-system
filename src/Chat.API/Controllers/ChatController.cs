using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Chat.API.ViewModels;
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
        }

        [HttpPost("send-message")]
        public async Task<ActionResult> SendMessageAsync(SendMessageRequest request)
        {
            var sendMessageDto = new SendMessageDto
            {
                Message = request.Message,
                SenderUserName = request.SenderUserName,
                TargetUserName = request.TargetUserName,
            };
            var res = await _chatService.SendMessageAsync(sendMessageDto);

            return Ok(res);
        }

        /// <summary>
        /// Both sent and received messages
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="numberOfMessages"></param>
        /// <returns></returns>
        [HttpGet("load-messages/{userName}")]
        public async Task<ActionResult<IEnumerable<ChatMessageDto>>> LoadMessages([FromRoute] string userName,
            [FromQuery] int? numberOfMessages = null)
        {
            var messages = await _chatService.LoadMessagesByCount(userName, numberOfMessages);

            return Ok(messages);
        }

        /// <summary>
        /// Only received messages
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="numberOfMessages"></param>
        /// <returns></returns>
        [HttpGet("load-received-messages/{userName}")]
        public async Task<ActionResult<IEnumerable<ChatMessageDto>>> LoadReceivedMessages([FromRoute] string userName,
            [FromQuery] int? numberOfMessages = null)
        {
            var messages = await _chatService.LoadMessagesByCount(userName, numberOfMessages);
            messages = messages.Where(x => x.TargetUserName == userName);

            return Ok(messages);
        }

        /// <summary>
        /// Only sent messages
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="numberOfMessages"></param>
        /// <returns></returns>
        [HttpGet("load-sent-messages/{userName}")]
        public async Task<ActionResult<IEnumerable<ChatMessageDto>>> LoadSentMessages([FromRoute] string userName,
            [FromQuery] int? numberOfMessages = null)
        {
            var messages = await _chatService.LoadMessagesByCount(userName, numberOfMessages);
            messages = messages.Where(x => x.SenderUserName == userName);

            return Ok(messages);
        }

        [HttpGet("load-messages-by-time/{userName}")]
        public async Task<ActionResult<IEnumerable<ChatMessageDto>>> LoadMessagesByTime([FromRoute] string userName,
            [FromQuery] DateTime dateTime)
        {
            var messages = await _chatService.LoadMessagesByTime(userName, dateTime);

            return Ok(messages);
        }

        [HttpGet("load-messages-from-startup-time/{userName}")]
        public async Task<ActionResult<IEnumerable<ChatMessageDto>>> LoadMessagesFromStartupTime([FromRoute] string
            userName, [FromQuery] DateTime startupTime)
        {
            // var applicationStartTime = Process.GetCurrentProcess().StartTime;
            var messages = await _chatService.LoadMessagesByTime(userName, startupTime);

            return Ok(messages);
        }

        [HttpGet("messages/{id}")]
        public async Task<ActionResult<ChatMessageDto>> GetByChatId(long id)
        {
            return await _chatService.GetChatById(id);
        }
    }
}