using FasleSafar.Models;

namespace FasleSafar.Data.Repositories
{
    public interface IAttractionRep
    {
        public List<Attraction> GetAllAttractions();
        public List<Attraction> GetAttractionsForPages(int skip);
        public List<Attraction> GetAttractionsOfDestination(int destinationId);
        public List<Attraction> GetAttractionsOfDestinationForPages(int destinationId, int skip);
        public Attraction GetAttractionById(int attractionId);
        public void AddAttraction(Attraction attraction);
        public void EditAttraction(Attraction attraction);
        public void RemoveAttraction(Attraction attraction);
        public void RemoveAttraction(int attractionId);
        public bool ExistAttraction(int attractionId);
    }
}
