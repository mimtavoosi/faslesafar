using FasleSafar.Models;

namespace FasleSafar.Data.Repositories
{
    public interface ITourRep
    {
        public List<Tour> GetAllTours();
        public List<Tour> GetFavoriteTours();
        public List<Tour> GetOpenTours();
		public List<Tour> GetIranTours();
		public List<Tour> GetIranToursForPages(int skip);
		public List<Tour> GetWorldTours();
		public List<Tour> GetWorldToursForPages(int skip);
		public List<Tour> GetToursForPages(int skip);
        public List<Tour> GetFavoriteToursForPages(int skip);
        public List<Tour> GetOpenToursForPages(int skip);
        public List<Tour> GetToursOfUser(int userId);
        public List<Tour> GetToursOfDestination(int destinationId);
        public List<Tour> GetToursOfDestinationForPages(int destinationId, int skip);
        public List<Tour> GetOpenToursOfDestination(int destinationId);
        public List<Tour> GetOpenToursOfDestinationForPages(int destinationId, int skip);
        public List<Tour> GetToursOfUserForPages(int userId, int skip);
        public Tour GetTourById(int tourId);
        public HotelStaring GetHotelStaringById(int staringId);
        public void AddTour(Tour tour);
        public void EditTour(Tour tour);
        public void RemoveTour(Tour tour);
        public void RemoveTour(int tourId);
        public bool ExistTour(int tourId);
        public StaringVM[] GetHotelStaringsOfTour(int tourId);
        public void PutPricesOfTour(int tourId,string title1,string title2,string title3,string price3,string price4,string price5);
        public List<HotelStaring> GetHotelStaringsListOfTour(int tourId);
        public void AddRating(RatingHistory rating);
        public bool ExistRating(int userId,int tourId);
    }
}
