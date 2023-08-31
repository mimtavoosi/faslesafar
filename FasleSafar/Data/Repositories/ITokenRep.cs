using FasleSafar.Models;

namespace FasleSafar.Data.Repositories
{
    public interface ITokenRep
	{
		public Token GetToken(string token);
		public string GetCookieToken();
		public void AddToken(string token, string type);
		public bool TokenIsValid(string token);
		public void MakeTokenExpire(string token);
		public bool ExistToken(string token);
		public void RemoveToken(Token token);

	}
}
