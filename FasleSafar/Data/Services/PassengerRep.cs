using FasleSafar.Data.Repositories;
using FasleSafar.Models;
using Microsoft.EntityFrameworkCore;

namespace FasleSafar.Data.Services
{
    public class PassengerRep: IPassengerRep
	{
        private FasleSafarContext _context;
        public PassengerRep(FasleSafarContext context)
        {
            _context = context;
        }

		public void AddPassenger(Passenger passenger)
		{
			_context.Passengers.Add(passenger);
			_context.SaveChanges();
			_context.Entry(passenger).State = EntityState.Detached;
		}

		public void AddPassengersForOrder(List<Passenger> passengers)
		{
			foreach (Passenger passenger in passengers)
			{
				AddPassenger(passenger);
			}
		}

		public List<Passenger> GetPassengersOfOrder(int orderId)
		{
			return _context.Passengers.Where(p => p.OrderId == orderId).ToList();
		}

		public void RemovePassenger(Passenger passenger)
		{
			_context.Passengers.Remove(passenger);
			_context.SaveChanges();
			_context.Entry(passenger).State = EntityState.Detached;
		}

		public void RemovePassengersOfOrder(int orderId)
		{
			var passengers = GetPassengersOfOrder(orderId);
			foreach (Passenger passenger in passengers)
			{
				RemovePassenger(passenger);
			}
		}
	}
}
