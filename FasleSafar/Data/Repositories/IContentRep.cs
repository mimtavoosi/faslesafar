using FasleSafar.Models;

namespace FasleSafar.Data.Repositories
{
    public interface IContentRep
    {
        public List<Content> GetAllContents();
        public List<Content> GetSiteInfo();
        public List<Content> GetContentsForMenu();
        public List<Content> GetServices();
        public List<Content> GetContentsForPages(int skip);
        public Content GetContentById(int contentId);
        public void AddContent(Content content);
        public void EditContent(Content content);
        public void RemoveContent(Content content);
        public void RemoveContent(int contentId);
        public bool ExistContent(int contentId);
    }
}
