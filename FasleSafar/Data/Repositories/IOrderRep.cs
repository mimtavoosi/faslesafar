using FasleSafar.Models;

namespace FasleSafar.Data.Repositories
{
    public interface IOrderRep
    {
        public List<Order> GetAllOrders();
        public List<Order> GetAllPaidOrders();
        public List<Order> GetOrdersForPages(int skip);
        public List<Order> GetOrdersByUserId(int userId);
		public List<Order> GetOrdersByTourId(int tourId);  
        public List<Order> GetPaidOrdersByUserId(int userId);
		public List<Order> GetPaidOrdersByTourId(int tourId);
		public List<Order> GetOrdersOfUserForPages(int userId, int skip);
        public Order GetCurrentOrderByUserId(int userId);
        public Order GetOrderById(int orderId);
        public void AddOrder(Order order);
        public void EditOrder(Order order);
        public void RemoveOrder(Order order);
        public void RemoveOrder(int orderId);
        public bool ExistOrder(int orderId);
        public void SetOrderFinally(Order order);
    }
}
