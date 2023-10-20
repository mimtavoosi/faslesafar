using FasleSafar.Models;

namespace FasleSafar.Data.Repositories
{
    public interface IPassengerRep
	{
		public List<Passenger> GetPassengersOfOrder(int orderId);
		public void RemovePassenger(Passenger passenger);
		public void AddPassengersForOrder(List<Passenger> passengers);
		public void RemovePassengersOfOrder(int orderId);
	}
}
