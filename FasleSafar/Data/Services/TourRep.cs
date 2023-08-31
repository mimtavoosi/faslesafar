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

        public HotelStaring GetHotelStaringById(int staringId)
        {
            return _context.HotelStarings.AsNoTracking().SingleOrDefault(h=> h.StaringId == staringId);
        }

        public List<HotelStaring> GetHotelStaringsListOfTour(int tourId)
        {
           return _context.HotelStarings.AsNoTracking().Where(h=> h.TourId == tourId && h.Price > 0).ToList();
        }

        public StaringVM[] GetHotelStaringsOfTour(int tourId)
        {
            StaringVM[] hotelStarings = new StaringVM[3];
            hotelStarings[0] = new StaringVM();
            hotelStarings[1] = new StaringVM();
            hotelStarings[2] = new StaringVM();
            hotelStarings[0].Title = "";
            hotelStarings[0].Price = "0";
            hotelStarings[1].Title = "";
            hotelStarings[1].Price = "0";
            hotelStarings[2].Title = "";
            hotelStarings[2].Price = "0";
            var starings = _context.HotelStarings.AsNoTracking().Where(h => h.TourId == tourId).ToList();
            hotelStarings[0].Price = starings[0].Price.Value.FixPrice();
            hotelStarings[1].Price = starings[1].Price.Value.FixPrice();
            hotelStarings[2].Price = starings[2].Price.Value.FixPrice();
            hotelStarings[0].Title = starings[0].Title;
            hotelStarings[1].Title = starings[1].Title;
            hotelStarings[2].Title = starings[2].Title;
            return hotelStarings;
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

		public void PutPricesOfTour(int tourId, string title1, string title2, string title3, string price3, string price4, string price5)
        {
            _context.HotelStarings.AsNoTracking().Where(h => h.TourId ==tourId).ToList().ForEach(h => _context.HotelStarings.Remove(h));
            _context.SaveChanges();
            List<StaringVM> prices = new List<StaringVM>();
            if (string.IsNullOrEmpty(price3)) price3 = "0";
            if (string.IsNullOrEmpty(price4)) price4 = "0";
            if (string.IsNullOrEmpty(price5)) price5 = "0";
            prices.Add(new StaringVM()
            {
                Title = title1??"کمترین قیمت",
                Price = price3
            });
            prices.Add(
                new StaringVM()
                {
                    Title = title2 ?? "قیمت متوسط",
                    Price = price4
                });
            prices.Add(new StaringVM()
            {
                Title = title3 ?? "بیشترین قیمت",
                Price = price5
            });
            foreach (StaringVM item in prices)
            {
                _context.HotelStarings.Add(new HotelStaring() {TourId = tourId,  Price = RetreivePrice(item.Price), Title = item.Title});
            }
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

        private decimal RetreivePrice(string finalPrice)
        {
            for (int i = 0; i < finalPrice.Length; i++)
            {
                if (finalPrice[i] == ',') finalPrice.Remove(i, 1);
            }
            return decimal.Parse(finalPrice);
        }
    }
}
