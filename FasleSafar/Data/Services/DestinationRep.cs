using FasleSafar.Data.Repositories;
using FasleSafar.Models;
using Microsoft.EntityFrameworkCore;

namespace FasleSafar.Data.Services
{
    public class DestinationRep: IDestinationRep
    {
        private FasleSafarContext _context;
        public DestinationRep(FasleSafarContext context)
        {
            _context = context;
        }

        public void AddDestination(Destination destination)
        {
            _context.Destinations.Add(destination);
            _context.SaveChanges();
            _context.Entry(destination).State = EntityState.Detached;
        }

        public void EditDestination(Destination destination)
        {
            _context.Destinations.Update(destination);
            _context.SaveChanges();
            _context.Entry(destination).State = EntityState.Detached;
        }

        public bool ExistDestination(int destinationId)
        {
           return _context.Destinations.Any(d => d.DestinationId == destinationId);
        }

        public List<Destination> GetAllDestinations()
        {
            return _context.Destinations.OrderByDescending(d => d.DestinationId).ToList();
        }

        public Destination GetDestinationById(int destinationId)
        {
            return _context.Destinations.Find(destinationId);
        }

        public List<Destination> GetDestinationsForPages(int skip)
        {
            return _context.Destinations.OrderByDescending(d => d.DestinationId).Skip(skip).Take(20).ToList();
        }

        public void RemoveDestination(Destination destination)
        {
            _context.Tours.Where(t => t.DestinationId == destination.DestinationId).ToList().ForEach(d => _context.Tours.Remove(d));
            _context.Destinations.Remove(destination);
            _context.SaveChanges();
            _context.Entry(destination).State = EntityState.Detached;
        }

        public void RemoveDestination(int destinationId)
        {
           var destination = GetDestinationById(destinationId);
            RemoveDestination(destination);
        }
    }
}
