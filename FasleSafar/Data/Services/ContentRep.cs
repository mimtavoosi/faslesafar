using FasleSafar.Data.Repositories;
using FasleSafar.Models;
using Microsoft.EntityFrameworkCore;

namespace FasleSafar.Data.Services
{
    public class ContentRep: IContentRep
    {
        private FasleSafarContext _context;
        public ContentRep(FasleSafarContext context)
        {
            _context = context;
        }

        public void AddContent(Content content)
        {
            _context.Contents.Add(content);
            _context.SaveChanges();
            _context.Entry(content).State = EntityState.Detached;
        }

        public void EditContent(Content content)
        {
            _context.Contents.Update(content);
            _context.SaveChanges();
            _context.Entry(content).State = EntityState.Detached;
        }

        public bool ExistContent(int contentId)
        {
           return _context.Contents.Any(x => x.ContentId == contentId);
        }

        public List<Content> GetAllContents()
        {
            return _context.Contents.Where(c=> c.ContentType != "اطلاعات").OrderByDescending(c=>c.ContentId).ToList();
        }

        public Content GetContentById(int contentId)
        {
            return _context.Contents.Find(contentId);
        }

        public List<Content> GetContentsForMenu()
        {
            return _context.Contents.Where(c => c.ContentType == "محتوا").ToList();
        }

        public List<Content> GetContentsForPages(int skip)
        {
            return _context.Contents.Where(c => c.ContentType != "اطلاعات").OrderByDescending(c => c.ContentId).Skip(skip).Take(21).ToList();
        }

        public List<Content> GetServices()
        {
            return _context.Contents.Where(c => c.ContentType == "خدمات").ToList();
        }

        public List<Content> GetSiteInfo()
        {
            return _context.Contents.Where(c => c.ContentType == "اطلاعات").ToList();
        }

        public void RemoveContent(Content content)
        {
            _context.Contents.Remove(content);
            _context.SaveChanges();
            _context.Entry(content).State = EntityState.Detached;
        }

        public void RemoveContent(int contentId)
        {
            var content = GetContentById(contentId);
            RemoveContent(content);
        }
    }
}
