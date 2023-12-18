using FasleSafar.Data.Repositories;
using FasleSafar.Models;
using FasleSafar.Utilities;
using Microsoft.EntityFrameworkCore;

namespace FasleSafar.Data.Services
{
    public class TourRep : ITourRep
    {
        private FasleSafarContext _context;
        public TourRep(FasleSafarContext context)
        {
            _context = context;
        }

        public void AddRating(RatingHistory rating)
        {
           _context.RatingHistories.Add(rating);
            _context.SaveChanges();
			_context.Entry(rating).State = EntityState.Detached;
		}

		public void AddTour(Tour tour)
        {
            _context.Tours.Add(tour);
            _context.SaveChanges();
            _context.Entry(tour).State = EntityState.Detached;
        }

        public void EditTour(Tour tour)
        {
            _context.Tours.Update(tour);
            _context.SaveChanges();
            _context.Entry(tour).State = EntityState.Detached;
        }

        public bool ExistRating(int userId, int tourId)
        {
           return _context.RatingHistories.AsNoTracking().Any(r=> r.UserId == userId && r.TourId == tourId);
        }

        public bool ExistTour(int tourId)
        {
           return _context.Tours.AsNoTracking().Any(t=> t.TourId == tourId);
        }

        public List<Tour> GetAllTours()
        {
            return _context.Tours.AsNoTracking().Include(d => d.Destination).OrderByDescending(t => t.TourId).ToList();
        }

        public List<Tour> GetFavoriteTours()
        {
            return _context.Tours.AsNoTracking().Include(d => d.Destination).Where(t=> t.OpenState == "فعال").OrderByDescending(t => t.AvgScore).ToList();
        }

        public List<Tour> GetFavoriteToursForPages(int skip)
        {
            return _context.Tours.AsNoTracking().Include(d => d.Destination).Where(t => t.OpenState == "فعال").OrderByDescending(t => t.AvgScore).Skip(skip).Take(20).ToList();
        }

        public string GetFirstPriceOfTour(int tourId)
        {
            string firstPrice = "0";
            var prices = GetHotelStaringsListOfTour(tourId);
            if (prices.Count > 0)
            {
                firstPrice = prices.Min(p => p.AdultPrice.RetreivePrice()).FixPrice();
            }
            return firstPrice;
        }

        public HotelStaring GetHotelStaringById(int staringId)
        {
            return _context.HotelStarings.AsNoTracking().SingleOrDefault(h=> h.StaringId == staringId);
        }

        public List<HotelStaring> GetHotelStaringsListOfTour(int tourId)
        {
           return _context.HotelStarings.AsNoTracking().Where(h=> h.TourId == tourId).ToList();
        }

   

		public List<Tour> GetIranTours()
		{
			return GetOpenTours().Where(t=> t.TourType == "داخلی").ToList();
		}

		public List<Tour> GetIranToursForPages(int skip)
		{
			return GetIranTours().Skip(skip).Take(20).ToList();
		}

		public List<Tour> GetOpenTours()
        {
            return _context.Tours.AsNoTracking().Include(d => d.Destination).Where(t => t.OpenState == "فعال").OrderByDescending(t => t.TourId).ToList();
        }

        public List<Tour> GetOpenToursForPages(int skip)
        {
          return GetOpenTours().Skip(skip).Take(20).ToList();
        }

        public List<Tour> GetOpenToursOfDestination(int destinationId)
        {
            return _context.Tours.AsNoTracking().Include(d => d.Destination).Where(t => t.DestinationId == destinationId && t.OpenState =="فعال").OrderByDescending(t => t.TourId).ToList();
        }

        public List<Tour> GetOpenToursOfDestinationForPages(int destinationId, int skip)
        {
            var tods = GetOpenToursOfDestination(destinationId);
            return tods.Skip(skip).Take(20).ToList();
        }

        public Tour GetTourById(int tourId)
        {
            return _context.Tours.AsNoTracking().Include(d => d.Destination).SingleOrDefault(t=> t.TourId == tourId);
        }

        public List<Tour> GetToursForPages(int skip)
        {
            return _context.Tours.AsNoTracking().Include(d => d.Destination).OrderByDescending(t => t.TourId).Skip(skip).Take(20).ToList();
        }

        public List<Tour> GetToursOfDestination(int destinationId)
        {
            return _context.Tours.AsNoTracking().Include(d => d.Destination).Where(t => t.DestinationId == destinationId).OrderByDescending(t => t.TourId).ToList();
        }

        public List<Tour> GetToursOfDestinationForPages(int destinationId, int skip)
        {
            var tods = GetToursOfDestination(destinationId);
            return tods.Skip(skip).Take(20).ToList();
        }

        public List<Tour> GetToursOfUser(int userId)
        {
            List<Tour> tours = _context.Orders.AsNoTracking()
              .Where(o => o.IsFinaly != "لغو شده" && o.UserId == userId)
              .Select(ca => ca.Tour)/*.Include(to=> to.Destination)*/.OrderByDescending(t => t.TourId)
              .ToList().DistinctBy(t => t.TourId).ToList();
            foreach (Tour item in tours)
            {
                item.Destination = _context.Destinations.Find(item.DestinationId);
            }
           return tours;
        }

        public List<Tour> GetToursOfUserForPages(int userId, int skip)
        {
            var tous=GetToursOfUser(userId);
            return tous.Skip(skip).Take(20).ToList();
        }

		public List<Tour> GetWorldTours()
		{
			return GetOpenTours().Where(t => t.TourType == "خارجی").ToList();
		}

		public List<Tour> GetWorldToursForPages(int skip)
		{
			return GetWorldTours().Skip(skip).Take(20).ToList();
		}

        public void PutPricesOfTour(int tourId, List<HotelStaring> starings)
        {
            foreach (HotelStaring t in starings)
            {
                t.TourId = tourId;
                t.AdultPrice = string.IsNullOrEmpty(t.AdultPrice) ? "0" : t.AdultPrice;
                t.ChildPrice = string.IsNullOrEmpty(t.ChildPrice) ? "0" : t.ChildPrice;
                t.BabyPrice = string.IsNullOrEmpty(t.BabyPrice) ? "0" : t.BabyPrice;
            }
            var foundstarings = GetHotelStaringsListOfTour(tourId);
            _context.HotelStarings.RemoveRange(foundstarings);
            _context.UpdateRange(starings);
            _context.SaveChanges();
        }

        public void RemoveTour(Tour tour)
        {
            _context.Orders.AsNoTracking().Where(o => o.TourId == tour.TourId).Include(p=>p.Passengers).ToList().ForEach(o => _context.Orders.Remove(o));
            _context.HotelStarings.AsNoTracking().Where(h => h.TourId == tour.TourId).ToList().ForEach(h => _context.HotelStarings.Remove(h));
            _context.RatingHistories.AsNoTracking().Where(o => o.TourId == tour.TourId).ToList().ForEach(o => _context.RatingHistories.Remove(o));
            _context.Tours.Remove(tour);
            _context.SaveChanges();
			_context.Entry(tour).State = EntityState.Detached;

		}

		public void RemoveTour(int tourId)
        {
            var tour = GetTourById(tourId);
            RemoveTour(tour);
        }
    }
}
