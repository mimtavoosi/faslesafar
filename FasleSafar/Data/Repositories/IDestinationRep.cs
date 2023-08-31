using FasleSafar.Models;

namespace FasleSafar.Data.Repositories
{
    public interface IDestinationRep
    {
        public List<Destination> GetAllDestinations();
        public List<Destination> GetDestinationsForPages(int skip);
        public Destination GetDestinationById(int destinationId);
        public void AddDestination(Destination destination);
        public void EditDestination(Destination destination);
        public void RemoveDestination(Destination destination);
        public void RemoveDestination(int destinationId);
        public bool ExistDestination(int destinationId);
    }
}
