using FasleSafar.Data.Repositories;
using FasleSafar.Models;
using FasleSafar.Utilities;
using Microsoft.EntityFrameworkCore;

namespace FasleSafar.Data.Services
{
    public class UserRep:IUserRep
    {
        private FasleSafarContext _context;
        public UserRep(FasleSafarContext context)
        {
            _context = context;
        }

        public void AddUser(User user)
        {
            user.Password = user.Password.ToHash();
            user.IsAdmin = user.IsAdmin.ToHash();
           _context.Users.Add(user);
           _context.SaveChanges();
           _context.Entry(user).State = EntityState.Detached;
        }

        public void EditUser(User user)
        {
			user.Password = user.Password.ToHash();
			user.IsAdmin = user.IsAdmin.ToHash();
			_context.Users.Update(user);
            _context.SaveChanges();
            _context.Entry(user).State = EntityState.Detached;
        }

        public bool ExistEmail(string email, int userId = 0)
        {
            if (userId != 0) return _context.Users.Any(u => u.Email == email && u.UserId != userId);
            else return _context.Users.Any(c => c.Email == email);
        }

        public bool ExistMobileNumber(string mobileNumber, int userId = 0)
        {
            if (userId != 0) return _context.Users.Any(u => u.MobileNumber == mobileNumber && u.UserId != userId);
            else return _context.Users.Any(c => c.MobileNumber == mobileNumber);
        }

        public bool ExistUser(int userId)
        {
            return _context.Users.Any(u => u.UserId == userId);
        }

        public List<User> GetAllUsers()
        {
            var users = _context.Users.OrderByDescending(u => u.UserId).ToList();
            foreach (User item in users)
            {
                item.Password = item.Password?.ToUnHash();
                item.IsAdmin = item.IsAdmin?.ToUnHash();
            }
            return users;
        }

        public User GetUserById(int userId)
        {
          var user= _context.Users.Find(userId);
          user.Password = user.Password?.ToUnHash();
          user.IsAdmin = user.IsAdmin?.ToUnHash();
          return user;
        }

        public User GetUserByMobileNumber(string mobileNumber)
        {
            var user= _context.Users.SingleOrDefault(u=> u.MobileNumber == mobileNumber);
			user.Password = user.Password?.ToUnHash();
			user.IsAdmin = user.IsAdmin?.ToUnHash();
			return user;
		}

        public User GetUserByEmail(string email)
        {
            var user = _context.Users.SingleOrDefault(u => u.Email == email);
			user.Password = user.Password?.ToUnHash();
			user.IsAdmin = user.IsAdmin?.ToUnHash();
			return user;
		}

        public List<User> GetUsersForPages(int skip)
        {
            return GetAllUsers().Skip(skip).Take(20).ToList();
        }

        public void RemoveUser(User user)
        {
            _context.RatingHistories.Where(o => o.UserId == user.UserId).ToList().ForEach(o => _context.RatingHistories.Remove(o));
            _context.Orders.Where(o => o.UserId == user.UserId).ToList().ForEach(o => _context.Orders.Remove(o));
            _context.Users.Remove(user);
            _context.SaveChanges();
            _context.Entry(user).State = EntityState.Detached;
        }

        public void RemoveUser(int userId)
        {
           var user = GetUserById(userId);
           RemoveUser(user);
        }

        public User GetUserForLogin(string email, string password)
        {
            password =password.ToHash();
            var user = _context.Users.SingleOrDefault(u => u.Email == email && u.Password == password);
			user.Password = user.Password?.ToUnHash();
			user.IsAdmin = user.IsAdmin?.ToUnHash();
			return user;
		}

        public bool CheckPassword(string email, string password)
        {
            password = password.ToHash();
            return _context.Users.Any(u => u.Email == email && u.Password == password);
        }

        public List<User> GetUsersOfTour(int tourId)
        {
             var users= _context.Orders.Include(o=> o.User)/*.Include(t=> t.Tour)*/
              .Where(d => d.TourId == tourId && d.IsFinaly != "لغو شده" && d.IsFinaly.Contains("منتظر پرداخت")==false)
              .Select(ca => ca.User).OrderByDescending(u => u.UserId).ToList();
			foreach (User item in users)
			{
				item.Password = item.Password?.ToUnHash();
				item.IsAdmin = item.IsAdmin?.ToUnHash();
			}
            return users;
		}

        public List<User> GetUsersOfTourForPages(int tourId, int skip)
        {
            var uots = GetUsersOfTour(tourId);
            var users = uots.Skip(skip).Take(20).ToList();
			foreach (User item in users)
			{
				item.Password = item.Password?.ToUnHash();
				item.IsAdmin = item.IsAdmin?.ToUnHash();
			}
			return users;
		}

		public void ChangePassword(int userId, string newPassword)
		{
		    var user=GetUserById(userId);
            user.Password = newPassword;
            EditUser(user);
		}
	}
}
