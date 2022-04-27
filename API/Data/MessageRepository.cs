using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public MessageRepository(DataContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Message> GetMessageAsync(int id)
        {
            return await _context.Messages
                .Include(u => u.Sender)
                .Include(u => u.Recipient)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public Task<PagedList<MessageDto>> GetMessagesForUserAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<MessageDto>> GetMessageThread(int currentUserId, int recipientId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        async Task<PagedList<MessageDto>> IMessageRepository.GetMessagesForUserAsync(MessageParams messageParams)
        {
            var query = _context.Messages
                .OrderByDescending(message => message.MessageSent)
                .AsQueryable(); // Converts an IEnumerable to an IQueryable.

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(user => user.Recipient.UserName == messageParams.Username
                && user.RecipientDeleted == false),
                "Outbox" => query.Where(user => user.Sender.UserName == messageParams.Username
                && user.SenderDeleted == false ),
                // default
                _ => query.Where(user => user.Recipient.UserName == messageParams.Username
                && user.DateRead == null
                && user.RecipientDeleted == false),
            };

            // want to return dto
            var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

            return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

         async Task<IEnumerable<MessageDto>> IMessageRepository.GetMessageThread(string currentUsername, string recipientUsername)
        {
            var messages =  await _context.Messages
                .Include(user => user.Sender).ThenInclude(p => p.Photos)
                .Include(user => user.Recipient).ThenInclude(p => p.Photos)
                .Where(
                message => message.Recipient.UserName == currentUsername
                        && message.RecipientDeleted == false
                        && message.Sender.UserName == recipientUsername
                        || message.Recipient.UserName == recipientUsername
                        && message.Sender.UserName == currentUsername
                        && message.SenderDeleted == false
                )
                .OrderByDescending(message => message.MessageSent)
                .ToListAsync();

            var unreadMessages = messages
                .Where(message => message.DateRead == null
                       && message.Recipient.UserName == currentUsername)
                .ToList();


            if(unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.Now;
                }

                await _context.SaveChangesAsync();
            }

            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }
    }
}