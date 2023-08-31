using FasleSafar.Data.Repositories;
using FasleSafar.Models;
using Microsoft.EntityFrameworkCore;

namespace FasleSafar.Data.Services
{
    public class ReqTripRep : IReqTripRep
    {
        private FasleSafarContext _context;
        public ReqTripRep(FasleSafarContext context)
        {
            _context = context;
        }

        public void AddReqTrip(ReqTrip reqTrip)
        {
            _context.ReqTrips.Add(reqTrip);
            _context.SaveChanges();
            _context.Entry(reqTrip).State = EntityState.Detached;
        }

        public void EditReqTrip(ReqTrip reqTrip)
        {
            _context.ReqTrips.Update(reqTrip);
            _context.SaveChanges();
            _context.Entry(reqTrip).State = EntityState.Detached;
        }

        public bool ExistReqTrip(int reqTripId)
        {
            return _context.ReqTrips.Any(o => o.ReqTripId == reqTripId);
        }

        public List<ReqTrip> GetReqTripsOfUserForPages(int userId, int skip)
        {
            var fous =GetReqTripsByUserId(userId);
            return fous.Skip(skip).Take(20).ToList();
        }

        public List<ReqTrip> GetAllReqTrips()
        {
            return _context.ReqTrips.Include(o => o.User).OrderByDescending(o => o.ReqTripId).ToList();
        }
        public ReqTrip GetReqTripById(int reqTripId)
        {
            return _context.ReqTrips.Include(o => o.User).SingleOrDefault(o=> o.ReqTripId == reqTripId);
        }
        public List<ReqTrip> GetReqTripsByUserId(int userId)
        {
            return _context.ReqTrips.Where(o => o.UserId == userId).Include(u=>u.User).ToList();
        }

        public List<ReqTrip> GetReqTripsForPages(int skip)
        {
            return _context.ReqTrips.Include(o => o.User).OrderByDescending(o => o.ReqTripId).Skip(skip).Take(20).ToList();
        }

        public void RemoveReqTrip(ReqTrip reqTrip)
        {
            _context.ReqTrips.Remove(reqTrip);
            _context.SaveChanges();
            _context.Entry(reqTrip).State = EntityState.Detached;
        }

        public void RemoveReqTrip(int reqTripId)
        {
            var reqTrip = GetReqTripById(reqTripId);
            RemoveReqTrip(reqTrip);
        }

    }
}
