using API.DTOs;
using API.Extentions;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class MessagesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;

        public MessagesController(
            IUserRepository userRepository,
            IMessageRepository messageRepository)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
        }

        //api/messages
        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username = User.GetUserName();

            // check if username = recipient name
            if (username == createMessageDto.RecipientUsername.ToLower())
            {
                return BadRequest("You cannot send messages to yourself");
            }

            var sender = await _userRepository.GetUserByUserNameAsync(username);

            var recipient = await _userRepository.GetUserByUserNameAsync(createMessageDto.RecipientUsername);

            if (recipient == null)
            {
                return NotFound();
            }


        }
    }
}