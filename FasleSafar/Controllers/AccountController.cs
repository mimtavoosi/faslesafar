using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using FasleSafar.Data.Repositories;
using FasleSafar.Models;
using System.Security.Claims;
using FasleSafar.Utilities;
using FasleSafar.Data.Services;
using System.Security.Cryptography;
using OfficeOpenXml.Style;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace SendReceiptsDemo.Controllers
{
    public class AccountController : Controller
    {
        private IUserRep _userRep;
        private ITourRep _tourRep;
		private IDestinationRep _destinationRep;
        private ITokenRep _tokenRep;
        private IContentRep _contentRep;
		public AccountController(IUserRep userRep, ITourRep tourRep,IDestinationRep destinationRep,ITokenRep tokenRep,IContentRep contentRep)
        {
            _userRep = userRep;
            _tourRep = tourRep;
            _destinationRep = destinationRep;
            _tokenRep = tokenRep;
            _contentRep = contentRep;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }

            var user = _userRep.GetUserForLogin(loginVM.Email.ToLower(), loginVM.Password);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim("IsAdmin",user.IsAdmin)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            var properties = new AuthenticationProperties
            {
                IsPersistent = loginVM.RememberMe
            };

            HttpContext.SignInAsync(principal, properties);

            return Redirect("/");
        }

		public IActionResult ForgotPassword()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult ForgotPassword(ForgotPasswordVM forgotPasswordVM)
		{
			if (!ModelState.IsValid) 
			{
				return View(forgotPasswordVM); 
			}
			int userId = _userRep.GetUserByEmail(forgotPasswordVM.Email).UserId;
            string tokenText = GenerateResetToken(userId);
			_tokenRep.AddToken(tokenText,"Reset Password");
            string link = GenerateResetPasswordLink(tokenText);
			bool sentState = ToolBox.SendEmail(forgotPasswordVM.Email, "بازنشانی کلمه عبور وبسایت فصل سفر", MakeResetPasswordMessage(_userRep.GetUserByEmail(forgotPasswordVM.Email).FullName,link));
			ViewBag.success = sentState ? "Success" : "Failed";
			return View(forgotPasswordVM);
		}

		public IActionResult ResetPassword(string token)
		{
            if (!_tokenRep.TokenIsValid(token))
                return Redirect("/Home/Error");
            ResetPasswordVM resetPasswordVM = new ResetPasswordVM()
            {
               UserId = int.Parse(token.Split('-')[0]),
               Token = token
            };
			return View(resetPasswordVM);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult ResetPassword(ResetPasswordVM resetPasswordVM)
		{
			if (!ModelState.IsValid) 
			{
				return View(resetPasswordVM); 
			}
            _userRep.ChangePassword(resetPasswordVM.UserId,resetPasswordVM.NewPassword);
            _tokenRep.MakeTokenExpire(resetPasswordVM.Token);
			ViewBag.success = "Success";
			return View(resetPasswordVM);
		}

		public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/Account/Login");
        }

        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost] //Post Action For Send Data
        [ValidateAntiForgeryToken]
        public IActionResult Signup(SignupVM signup) //Input page model (RegisterViewModel) for refresh the page to show errors
        {
            if (!ModelState.IsValid) //If Input Data in form is invalid
            {
                return View(signup); //Show the page with Input RegisterViewModel to show errors
            }

            User user = new User()
            {
                Email = signup.Email.ToLower(), //tolower easy to check
                Password = signup.Password,
                FullName = signup.FullName,
                MobileNumber = signup.MobileNumber,
                IsAdmin = "کاربر"
            };
            _userRep.AddUser(user); //Add instance to DB

            return Redirect("/Account/Login");
        }

        public async Task<IActionResult> EditInfo()
        {
            int id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (id == 0 || _userRep == null)
            {
                return NotFound();
            }

            SignupVM UserVM = _userRep.GetAllUsers().Where(c => c.UserId == id).Select(s => new SignupVM()
            {
                UserId = s.UserId,
                Email = s.Email,
                Password = s.Password,
                RePassword = s.Password,
                FullName = s.FullName,
                MobileNumber = s.MobileNumber,
                IsAdmin = s.IsAdmin

            }).FirstOrDefault();
            if (UserVM == null)
            {
                return NotFound();
            }
            return View(UserVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditInfo(SignupVM user)
        {
            int id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                User theUser = new User()
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    FullName = user.FullName,
                    IsAdmin = user.IsAdmin,
                    MobileNumber = user.MobileNumber,
                    Password = user.Password
                };
                _userRep.EditUser(theUser);
            }
            return View(user);
        }
		private void initSearchVals()
		{
			ViewBag.IranToursCount = _tourRep.GetIranTours().Count;
			ViewBag.WorldToursCount = _tourRep.GetWorldTours().Count;
			ViewBag.Destinations = _destinationRep.GetAllDestinations();
			ViewBag.Dates = ExtractDates();
		}



		private List<DateVM> ExtractDates()
		{
			List<DateVM> dates = _tourRep.GetOpenTours().Select(d => new DateVM()
			{
				DbDate = d.StartDate,
				ShowDate = DispDate(d.StartDate)
			}).ToList();
			dates = dates.DistinctBy(d => d.DbDate).OrderByDescending(d => d.DbDate).ToList();
			return dates;
		}
		private string DispDate(string startDate)
		{
			string[] arr = startDate.Split('/');
			return $"{arr[2]} {GetMonthName(arr[1])} {arr[0]}";

		}

		private string GetMonthName(string v)
		{
			switch (v)
			{
				case "1":
				default:
					return "فروردین";
				case "2":
					return "اردیبهشت";
				case "3":
					return "خرداد";
				case "4":
					return "تیر";
				case "5":
					return "مرداد";
				case "6":
					return "شهریور";
				case "7":
					return "مهر";
				case "8":
					return "آبان";
				case "9":
					return "آذر";
				case "10":
					return "دی";
				case "11":
					return "بهمن";
				case "12":
					return "اسفند";
			}
		}

		private string MakeResetPasswordMessage(string fullName, string link)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine($"{fullName} عزیز! <br />");
			sb.AppendLine($"برای بازنشانی کامه عبور خود روی لینک زیر کلیک کنید <br />");
			sb.AppendLine($"<a href=\"{link}\" target=\"_parent\">بازنشانی کلمه عبور</a><br />");
			sb.AppendLine($"یا لینک زیر را داخل نوار آدرس مرورگر قرار دهید <br />");
			sb.AppendLine($"{link}<br />");
			sb.AppendLine($"گفتنی است که این لینک در طی دو ساعت آینده منقضی خواهد شد <br />");
			sb.AppendLine($"{_contentRep.GetContentById(1030).ContentText}<br />");
			return sb.ToString();
		}

		public IActionResult ShowUserTours(int pageid = 1)
        {
			initSearchVals();
			int id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            int skip = (pageid - 1) * 20;
            SetCookie("UserToursSkip",skip.ToString());
            int Count = _tourRep.GetToursOfUser(id).Count();
            ViewBag.UserToursPageID = pageid;
            ViewBag.UserToursPageCount = Count / 20;
            ViewBag.ToursCount = Count;
            ViewBag.StartIndex = (Count > 0 ? ((pageid > 1) ? (20 * --pageid) + 1 : 1): 0);
            ViewBag.EndIndex = (Count > 20) ? 20 : Count;
            List<TourVM> tourVMs = _tourRep.GetToursOfUserForPages(id, skip).Select(t => new TourVM()
            {
                AvgScore = t.AvgScore,
                Capacity = t.Capacity,
                DaysCount = t.DaysCount,
                DestinationId = t.DestinationId,
                EndDate = t.EndDate,
                OpenState = t.OpenState,
                Price = _tourRep.GetFirstPriceOfTour(t.TourId),
                ScoreCount = t.ScoreCount,
                StartDate = t.StartDate,
                TotalScore = t.TotalScore,
                TourId = t.TourId,
                TourTitle = t.TourTitle,
                TourDescription = t.TourDescription,
                TourType = t.TourType,
                TransportType = t.TransportType,
                Destination = t.Destination,
                Vehicle = t.Vehicle,
                BigImage = t.BigImage,
                SmallImage = t.SmallImage,
                Attractions = t.Attractions,
                ExcludeCosts = t.ExcludeCosts,
                Facilities = t.Facilities,
                ImagesAlbum = t.ImagesAlbum,
                IncludeCosts = t.IncludeCosts,
                ReachTime = t.ReachTime,
                ReturnTime = t.ReturnTime,
                GeoCoordinates = t.GeoCoordinates,

            }).ToList();
            return View(tourVMs);
        }

		#region TokenManager

		private string GenerateResetToken(int userId)
		{
			return $"{userId}-{ToolBox.GenerateToken()}";
		}

		// ایجاد لینک بازنشانی کلمه عبور
		private string GenerateResetPasswordLink(string tokenText)
		{
			return Url.Action("ResetPassword", "Account", new { token = tokenText }, Request.Scheme);
		}

		#endregion

		#region AjaxMethods

		public IActionResult CheckPassword(string Email, string Password) // an Action that remoted for check the field value validation (no need to post page)
		{
			if (_userRep.CheckPassword(Email.ToLower(), Password))
			{
				return Json(true); // send true value
			}
			else return Json("نام کاربری یا کلمه عبور وارد شده نادرست است"); //send error text
		}
		public IActionResult isNewMobileNumber(string MobileNumber, int UserId = 0) // an Action that remoted for check the field value validation (no need to post page)
		{
			if (!_userRep.ExistMobileNumber(MobileNumber.ToLower(), UserId))
			{
				return Json(true); // send true value
			}
			else return Json($"شماره موبایل {MobileNumber} تکراری است"); //send error text
		}

		public IActionResult isNewEmailAddress(string Email, int UserId = 0) // an Action that remoted for check the field value validation (no need to post page)
		{
			if (!_userRep.ExistEmail(Email.ToLower(), UserId))
			{
				return Json(true); // send true value
			}
			else return Json($"پست الکترونیک {Email} تکراری است"); //send error text
		}
		public IActionResult ExistEmailAddress(string Email) // an Action that remoted for check the field value validation (no need to post page)
		{
			if (_userRep.ExistEmail(Email.ToLower()))
			{
				return Json(true); // send true value
			}
			else return Json($"پست الکترونیک {Email} در سیستم وجود ندارد"); //send error text
		}

		#endregion

		#region ManageCookies

		//public void SetCookie(string key, string value, bool isPersistant = false)
		//      {
		//          CookieOptions options = new CookieOptions() { IsEssential = true , HttpOnly=true };
		//          if (isPersistant) options.Expires = DateTime.Now.AddMinutes(20);
		//          Response.Cookies.Delete(key);
		//          Response.Cookies.Append(key, value, options);
		//      }

		//      public string GetCookie(string key)
		//      {
		//          return Request.Cookies[key].ToString();
		//      }

		public void SetCookie(string key, string value, bool isPersistant = false)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var keyBytes = Encoding.UTF8.GetBytes(_tokenRep.GetCookieToken());
			var signingKey = new SymmetricSecurityKey(keyBytes);

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
				{
			new Claim(key, value)
				}),
				Expires = isPersistant ? DateTime.UtcNow.AddMinutes(20) : DateTime.UtcNow.AddMinutes(1),
				SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			var tokenString = tokenHandler.WriteToken(token);

			var cookieOptions = new CookieOptions
			{
				IsEssential = true,
				HttpOnly = true,
				Secure = true, // If your site uses HTTPS
				SameSite = SameSiteMode.None // May need to adjust this based on your site's requirements
			};

			Response.Cookies.Delete(key);
			Response.Cookies.Append(key, tokenString, cookieOptions);
		}

		public string GetCookie(string key)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var keyBytes = Encoding.UTF8.GetBytes(_tokenRep.GetCookieToken());
			var signingKey = new SymmetricSecurityKey(keyBytes);

			var tokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = signingKey,
				ValidateIssuer = false, // Set these values based on your JWT configuration
				ValidateAudience = false // Set these values based on your JWT configuration
			};

			try
			{
				var token = Request.Cookies[key];
				var claimsPrincipal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
				var claim = claimsPrincipal.FindFirst(key);

				if (claim != null)
				{
					return claim.Value;
				}
			}
			catch (SecurityTokenException)
			{
				// Handle token validation error (e.g., expired or invalid token)
			}

			return null; // If the cookie is not found or validation fails
		}

		#endregion
	}
}
