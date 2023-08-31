using FasleSafar.Models;

namespace FasleSafar.Data.Repositories
{
    public interface IUserRep
    {
        public List<User> GetAllUsers();
        public List<User> GetUsersForPages(int skip);
        public List<User> GetUsersOfTour(int tourId);
        public List<User> GetUsersOfTourForPages(int tourId,int skip);
        public User GetUserById(int userId);
        public User GetUserByMobileNumber(string mobileNumber);
        public User GetUserByEmail(string email);
        public void AddUser(User user);
        public void EditUser(User user);
        public void RemoveUser(User user);
        public void RemoveUser(int userId);
        public bool ExistMobileNumber(string mobileNumber, int userId = 0);
        public bool ExistEmail(string email, int userId = 0);
        public bool ExistUser(int userId);
        public User GetUserForLogin(string email, string password);
        public bool CheckPassword(string email, string password);
		public void ChangePassword(int userId,string newPassword);
	}
}
