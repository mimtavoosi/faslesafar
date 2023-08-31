using FasleSafar.Data.Repositories;
using FasleSafar.Models;
using FasleSafar.Utilities;
using Microsoft.EntityFrameworkCore;

namespace FasleSafar.Data.Services
{
    public class TokenRep : ITokenRep
	{
        private FasleSafarContext _context;
        public TokenRep(FasleSafarContext context)
        {
            _context = context;
        }

		public void AddToken(string token,string type)
		{
			Token theToken=new Token()
			{ 
				TokenText = token,
				TokenDateTime = type =="Cookie" ? DateTime.UtcNow.AddMonths(1) : DateTime.UtcNow.AddHours(2),
				TokenType = type,
				TokenState = true
			};
			_context.Tokens.Add(theToken);
			_context.SaveChanges();
			_context.Entry(theToken).State = EntityState.Detached;
		}

		public bool ExistToken(string token)
		{
			return _context.Tokens.Any(t => t.TokenText == token);
		}

		public string GetCookieToken()
		{
			if (_context.Tokens.Any(t=> t.TokenType == "Cookie"))
			{
				TimeSpan difference = _context.Tokens.SingleOrDefault(t => t.TokenType == "Cookie").TokenDateTime - DateTime.UtcNow;
				if (difference.TotalDays < 1)
				{
					var token = _context.Tokens.SingleOrDefault(t => t.TokenType == "Cookie");
					RemoveToken(token);
					AddToken(ToolBox.GenerateToken(), "Cookie");
				}
			}
			else
			{
				AddToken(ToolBox.GenerateToken(), "Cookie");
			}
			return _context.Tokens.SingleOrDefault(t => t.TokenType == "Cookie").TokenText;
		}

		public Token GetToken(string token)
		{
			return _context.Tokens.SingleOrDefault(t=> t.TokenText == token);
		}

		public void MakeTokenExpire(string token)
		{
			var theToken = GetToken(token);
			theToken.TokenState = false;
			_context.Tokens.Update(theToken);
			_context.SaveChanges();
			_context.Entry(theToken).State = EntityState.Detached;
		}

		public void RemoveToken(Token token)
		{
			_context.Tokens.Remove(token);
			_context.SaveChanges();
			_context.Entry(token).State = EntityState.Detached;
		}

		public bool TokenIsValid(string token)
		{
			if (ExistToken(token))
			{
				var theToken = GetToken(token);
				if (theToken.TokenState && DateTime.UtcNow <= theToken.TokenDateTime)
					return true;
			}
			MakeTokenExpire(token);
			return false;
		}
	}
}
