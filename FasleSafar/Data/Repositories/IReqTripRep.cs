using FasleSafar.Models;

namespace FasleSafar.Data.Repositories
{
    public interface IReqTripRep
    {
        public List<ReqTrip> GetAllReqTrips();
        public List<ReqTrip> GetReqTripsForPages(int skip);
        public List<ReqTrip> GetReqTripsByUserId(int userId);
        public List<ReqTrip> GetReqTripsOfUserForPages(int userId, int skip);
        public ReqTrip GetReqTripById(int reqTripId);
        public void AddReqTrip(ReqTrip reqTrip);
        public void EditReqTrip(ReqTrip reqTrip);
        public void RemoveReqTrip(ReqTrip reqTrip);
        public void RemoveReqTrip(int reqTripId);
        public bool ExistReqTrip(int reqTripId);
    }
}
