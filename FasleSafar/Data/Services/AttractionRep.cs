using FasleSafar.Data.Repositories;
using FasleSafar.Models;
using Microsoft.EntityFrameworkCore;

namespace FasleSafar.Data.Services
{
    public class AttractionRep: IAttractionRep
    {
        private FasleSafarContext _context;
        public AttractionRep(FasleSafarContext context)
        {
            _context = context;
        }

        public void AddAttraction(Attraction attraction)
        {
            _context.Attractions.Add(attraction);
            _context.SaveChanges();
            _context.Entry(attraction).State = EntityState.Detached;
        }

        public void EditAttraction(Attraction attraction)
        {
            _context.Attractions.Update(attraction);
            _context.SaveChanges();
            _context.Entry(attraction).State = EntityState.Detached;
        }

        public bool ExistAttraction(int attractionId)
        {
           return _context.Attractions.Any(d => d.AttractionId == attractionId);
        }

        public List<Attraction> GetAllAttractions()
        {
            return _context.Attractions.AsNoTracking().Include(d=> d.Destination).OrderByDescending(d => d.AttractionId).ToList();
        }

        public Attraction GetAttractionById(int attractionId)
        {
            return _context.Attractions.Include(d => d.Destination).SingleOrDefault(a=> a.AttractionId == attractionId);
        }

        public List<Attraction> GetAttractionsForPages(int skip)
        {
            return GetAllAttractions().Skip(skip).Take(20).ToList();
        }

        public List<Attraction> GetAttractionsOfDestination(int destinationId)
        {
            return _context.Attractions.AsNoTracking().Include(d => d.Destination).Where(t => t.DestinationId == destinationId).OrderByDescending(t => t.AttractionId).ToList();
        }

        public List<Attraction> GetAttractionsOfDestinationForPages(int destinationId, int skip)
        {
            var aods = GetAttractionsOfDestination(destinationId);
            return aods.Skip(skip).Take(20).ToList();
        }

        public void RemoveAttraction(Attraction attraction)
        {
            _context.Attractions.Remove(attraction);
            _context.SaveChanges();
            _context.Entry(attraction).State = EntityState.Detached;
        }

        public void RemoveAttraction(int attractionId)
        {
           var attraction = GetAttractionById(attractionId);
            RemoveAttraction(attraction);
        }
    }
}
