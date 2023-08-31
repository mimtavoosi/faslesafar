using Microsoft.EntityFrameworkCore;
using FasleSafar.Data.Repositories;
using FasleSafar.Models;

namespace FasleSafar.Data.Services
{
    public class MessageRep:IMessageRep
    {
        private FasleSafarContext _context;
        public MessageRep(FasleSafarContext context)
        {
            _context = context;
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
            _context.SaveChanges();
            _context.Entry(message).State = EntityState.Detached;
        }
        public void EditMessage(Message message)
        {
            _context.Messages.Update(message);
            _context.SaveChanges();
            _context.Entry(message).State = EntityState.Detached;
        }

        public bool ExistMessage(int messageId)
        {
            return _context.Messages.Any(a => a.MessageId == messageId);
        }

        public Message GetMessageById(int messageId)
        {
            return _context.Messages.Include(m => m.User).SingleOrDefault(m=> m.MessageId == messageId);
        }

        public List<Message> GetAllMessages()
        {
            return _context.Messages.OrderByDescending(m => m.MessageId).Include(m=> m.User).ToList();
        }

        public List<Message> GetMessagesForPages(int skip)
        {
            return _context.Messages.Include(m => m.User).OrderByDescending(a => a.MessageId).Skip(skip).Take(20).ToList();
        }

        public void RemoveMessage(Message message)
        {
            _context.Messages.Remove(message);
            _context.SaveChanges();
            _context.Entry(message).State = EntityState.Detached;
        }

        public void RemoveMessage(int messageId)
        {
           Message message = GetMessageById(messageId);
           RemoveMessage(message);
           _context.Entry(message).State = EntityState.Detached;
        }

        public List<Message> GetMessagesByUserId(int userId)
        {
            return _context.Messages.Where(o => o.UserId == userId).Include(u => u.User).ToList();
        }

        public List<Message> GetMessagesOfUserForPages(int userId, int skip)
        {
            var mous = GetMessagesByUserId(userId);
            return mous.Skip(skip).Take(20).ToList();
        }
    }
}
