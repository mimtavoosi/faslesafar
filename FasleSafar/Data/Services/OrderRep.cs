using FasleSafar.Data.Repositories;
using FasleSafar.Models;
using Microsoft.EntityFrameworkCore;

namespace FasleSafar.Data.Services
{
    public class OrderRep:IOrderRep
    {
        private FasleSafarContext _context;
        public OrderRep(FasleSafarContext context)
        {
            _context = context;
        }

        public void AddOrder(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
            _context.Entry(order).State = EntityState.Detached;
        }

        public void EditOrder(Order order)
        {
            _context.Orders.Update(order);
            _context.SaveChanges();
			_context.Entry(order).State = EntityState.Detached;
		}

		public bool ExistOrder(int orderId)
        {
            return _context.Orders.Any(o => o.OrderId == orderId);
        }

        public List<Order> GetOrdersOfUserForPages(int userId, int skip)
        {
            var fous =GetOrdersByUserId(userId);
            return fous.Skip(skip).Take(20).ToList();
        }

        public List<Order> GetAllOrders()
        {
            return _context.Orders.Include(o => o.User).Include(o=> o.Tour).OrderByDescending(o => o.OrderId).ToList();
        }
         public List<Order> GetAllPaidOrders()
        {
            return _context.Orders.Where(o=> o.IsFinaly.Contains("پرداخت اقساطی") || o.IsFinaly.Contains("پرداخت نقدی")).Include(o => o.User).Include(o=> o.Tour).OrderByDescending(o => o.OrderId).ToList();
        }

        public Order GetCurrentOrderByUserId(int userId)
        {
            return _context.Orders.Where(o => o.UserId == userId && o.IsFinaly.Contains("منتظر پرداخت"))
                .Include(d => d.Tour).Include(u=> u.User).OrderByDescending(o=> o.OrderId ).FirstOrDefault();
        }

        public Order GetOrderById(int orderId)
        {
            return _context.Orders.Include(o => o.User).Include(t=> t.Tour).SingleOrDefault(o=> o.OrderId == orderId);
        }
        public List<Order> GetOrdersByUserId(int userId)
        {
            return _context.Orders.Where(o => o.UserId == userId)
                 .Include(d => d.Tour).Include(u=>u.User).ToList();
        }

		public List<Order> GetOrdersByTourId(int tourId)
		{
			return _context.Orders.Where(o => o.TourId == tourId)
				 .Include(d => d.Tour).Include(u => u.User).ToList();
		}
         public List<Order> GetPaidOrdersByUserId(int userId)
        {
            return _context.Orders.Where(o => o.UserId == userId && (o.IsFinaly.Contains("پرداخت اقساطی") || o.IsFinaly.Contains("پرداخت نقدی")))
                 .Include(d => d.Tour).Include(u=>u.User).ToList();
        }

		public List<Order> GetPaidOrdersByTourId(int tourId)
		{
			return _context.Orders.Where(o => o.TourId == tourId && (o.IsFinaly.Contains("پرداخت اقساطی") || o.IsFinaly.Contains("پرداخت نقدی")))
				 .Include(d => d.Tour).Include(u => u.User).ToList();
		}

		public List<Order> GetOrdersForPages(int skip)
        {
            return _context.Orders.Include(o => o.User).Include(o=> o.Tour).OrderByDescending(o => o.OrderId).Skip(skip).Take(20).ToList();
        }

        public void RemoveOrder(Order order)
        {
			_context.Passengers.Where(p => p.OrderId == order.OrderId).ToList().ForEach(p => _context.Passengers.Remove(p));
			_context.Orders.Remove(order);
            _context.SaveChanges();
            _context.Entry(order).State = EntityState.Detached;
        }

        public void RemoveOrder(int orderId)
        {
            var order = GetOrderById(orderId);
            RemoveOrder(order);
        }

        public void SetOrderFinally(Order order)
        {
            order.IsFinaly = "پرداخت نقدی";
            _context.Orders.Update(order);
            _context.SaveChanges();
        }
    }
}
