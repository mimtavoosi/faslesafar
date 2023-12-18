using FasleSafar.Data.Repositories;
using FasleSafar.Models;
using FasleSafar.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NuGet.Protocol.Plugins;
using Parbad.Gateway.Melli;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FasleSafar.Controllers
{
    [Authorize]
    public class CardController : Controller
    {
        public ITourRep _tourRep;
        public IOrderRep _orderRep;
        public IUserRep _userRep;
        public IContentRep _contentRep;
        public IPassengerRep _passengerRep;
        public ITokenRep _tokenRep;

        public CardController(ITourRep tourRep, IOrderRep orderRep,IUserRep userRep, IContentRep contentRep,IPassengerRep passengerRep,ITokenRep tokenRep)
        {
            _tourRep = tourRep;
            _orderRep = orderRep;
            _userRep = userRep;
            _contentRep = contentRep;
            _passengerRep = passengerRep;
            _tokenRep = tokenRep;
        }

        [HttpPost]
        public IActionResult ViewCard(int tourId, int priceId,string leasing,string factorreq)
        {
            decimal maliat = 0;
            decimal total = 0;
            decimal PriceForPay = 0;
            SetCookie("CurTourId", tourId.ToString());
            Tour tour = _tourRep.GetTourById(tourId);
            HotelStaring staring = _tourRep.GetHotelStaringById(priceId);


            ViewBag.Leasing = leasing ?? "off";

            if (ViewBag.Leasing == "on")
            {
                SetCookie("Leasing",true.ToString());
                int deposit = int.Parse(_contentRep.GetContentById(1018).ContentText);
                ViewBag.DepositPer= deposit;
                decimal finprice = (staring.AdultPrice.RetreivePrice() * deposit) / 100;
				PriceForPay= finprice;
				ViewBag.FinPrice = finprice.FixPrice();
            }
            else
            {
                SetCookie("Leasing", false.ToString());
                PriceForPay = staring.AdultPrice.RetreivePrice();
            }


			ViewBag.FactorReq = factorreq ?? "off";

			if (ViewBag.FactorReq == "on")
			{
				SetCookie("FactorReq", true.ToString());
				int maliatPercent = int.Parse(_contentRep.GetContentById(1023).ContentText);
				ViewBag.MaliatPer = maliatPercent;
				maliat = (staring.AdultPrice.RetreivePrice() * maliatPercent) / 100;
			}

			else
			{
				SetCookie("FactorReq", false.ToString());
			}

			total = maliat + PriceForPay;

			TourVM tourVM = new TourVM()
            {
                AvgScore = tour.AvgScore,
                Capacity = tour.Capacity,
                DaysCount = tour.DaysCount,
                DestinationId = tour.DestinationId,
                EndDate = tour.EndDate,
                OpenState = tour.OpenState,
                Price = staring.AdultPrice,
                ScoreCount = tour.ScoreCount,
                StartDate = tour.StartDate,
                TotalScore = tour.TotalScore,
                TourId = tour.TourId,
                TourTitle = tour.TourTitle,
                TourDescription = tour.TourDescription,
                TourType = tour.TourType,
                TransportType = tour.TransportType,
                Destination = tour.Destination,
                Vehicle = tour.Vehicle,
                IsLeasing = tour.IsLeasing,
                BigImage = tour.BigImage,
                SmallImage = tour.SmallImage,
                Attractions = tour.Attractions,
                ExcludeCosts = tour.ExcludeCosts,
                Facilities = tour.Facilities,
                ImagesAlbum = tour.ImagesAlbum,
                IncludeCosts = tour.IncludeCosts,
                ReachTime = tour.ReachTime,
                ReturnTime = tour.ReturnTime,
                GeoCoordinates = tour.GeoCoordinates,
            };
            ViewBag.PriceId = priceId; 
            ViewBag.Maliat = maliat.FixPrice();
            ViewBag.TotalPrice = total.FixPrice();
            ViewBag.Tour = tourVM;
            int ruleType = tourVM.TourType.Contains("خارجی") ? 1029 : 1028;
            ViewBag.Rules = _contentRep.GetContentById(ruleType).ContentText;
			return View();
        }

        [HttpPost]
        public IActionResult BuyTour(FactorPricesVM factor, string data)
        {
			try
			{
				List<Passenger> passengers = JsonConvert.DeserializeObject<List<Passenger>>(data);
				decimal adultfinprice = 0, childfinprice = 0, babyfinprice;
				HotelStaring staring = _tourRep.GetHotelStaringById(factor.PriceId);
            decimal maliatval = decimal.Parse(_contentRep.GetContentById(1023).ContentText);
			var maliatPrc = Convert.ToDecimal((maliatval / 100));

          
				if (!bool.Parse(GetCookie("Leasing")))
				{
					adultfinprice = (staring.AdultPrice.RetreivePrice());
					childfinprice = (staring.ChildPrice.RetreivePrice());
					babyfinprice = (staring.BabyPrice.RetreivePrice());
				}
				else
				{
					int deposit = int.Parse(_contentRep.GetContentById(1018).ContentText);
					adultfinprice = (staring.AdultPrice.RetreivePrice() * deposit) / 100;
					childfinprice = (staring.ChildPrice.RetreivePrice() * deposit) / 100;
					babyfinprice = (staring.BabyPrice.RetreivePrice() * deposit) / 100;
				}

				factor.OnePrice = ((factor.AdultCount * staring.AdultPrice.RetreivePrice()) + (factor.ChildCount * staring.ChildPrice.RetreivePrice()) + (factor.BabyCount * staring.BabyPrice.RetreivePrice())).FixPrice();
				factor.FinPrice = ((factor.AdultCount * adultfinprice) + (factor.ChildCount * childfinprice) + (factor.BabyCount * babyfinprice)).FixPrice();
				factor.Maliat = bool.Parse(GetCookie("FactorReq")) ? (factor.FinPrice.RetreivePrice() * maliatPrc).FixPrice() : "0";
				factor.TotalPrice = (factor.FinPrice.RetreivePrice() + factor.Maliat.RetreivePrice()).FixPrice();

				Order order = new Order()
				{
					CreateDate = DateTime.Now.ToString("yyyy/MM/dd - HH:mm").ToShamsi(),
					IsFinaly = bool.Parse(GetCookie("Leasing")) ? "منتظر پرداخت اقساطی" : "منتظر پرداخت نقدی",
					UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                    FactorRequest = bool.Parse(GetCookie("FactorReq")),
					AdultCount = factor.AdultCount,
					TourId = factor.TourId,
					ChildCount = factor.ChildCount,
					BabyCount = factor.BabyCount,
					Price = factor.TotalPrice
				};
				_orderRep.AddOrder(order);

				string passengersLog = $"ثبت سفارش: تعداد مسافران سفارش شماره {order.OrderId} {passengers.Count} می باشد \n log 9";
				ToolBox.SaveLog(passengersLog);


				foreach (Passenger item in passengers)
				{
					item.SpecialDisease = item.SpecialDisease ?? "";
					item.EducationLevel = item.EducationLevel ?? "";
					item.Job = item.Job ?? "";
					item.OrderId = order.OrderId;
					item.BirthDate = item.BirthDate.FixDateToSave();
				}
				_passengerRep.AddPassengersForOrder(passengers);
			}
            catch (Exception ex)
            {

                ToolBox.SaveLog(ex.Message + '\n' + ex.InnerException?.Message + "\n log 13");
            }
		
            return Redirect("/Payment/Pay");
        }


        public IActionResult FinishBuy(int id,string message="",string refId = "0")
        {
          var order = _orderRep.GetOrderById(id);
          var user = _userRep.GetUserById((int)order.UserId);
          var tour = _tourRep.GetTourById((int) order.TourId);
          ViewBag.OrderId =order.OrderId;
          ViewBag.Message = message;
          ViewBag.TourTitle = tour.TourTitle;
          ViewBag.AdultCount = order.AdultCount;
          ViewBag.ChildCount = order.ChildCount;
		  ViewBag.BabyCount = order.BabyCount;
	      ViewBag.Price = order.Price;
          ViewBag.UserName = user.FullName;
          ViewBag.Email = user.Email;
          ViewBag.MobileNumber = user.MobileNumber;
          ViewBag.PayMode = order.IsFinaly;
          ViewBag.Deposit = order.IsFinaly.Contains("اقساطی");
          ViewBag.FactorRequest = order.FactorRequest.ToString();
            if (refId != "0")
            {
                ViewBag.RefId = refId;
                ViewBag.Status = "موفق";
            }
         else ViewBag.Status = "ناموفق";
            return View();
        }

       

        [HttpPost]
        public async Task<FactorPricesVM> ChangePrice(int adult, int child,int baby,int priceId)
        {
            decimal adultfinprice = 0, childfinprice=0, babyfinprice;
			if (adult == 0) return null;
            FactorPricesVM factor = new FactorPricesVM() {
                AdultCount = adult,
                BabyCount = baby,
			    ChildCount = child
            };
            HotelStaring tour = _tourRep.GetHotelStaringById(priceId);
			decimal maliatval = decimal.Parse(_contentRep.GetContentById(1023).ContentText);
			var maliatPrc = Convert.ToDecimal((maliatval / 100));

			if (!bool.Parse(GetCookie("Leasing")))
            {
                adultfinprice = (tour.AdultPrice.RetreivePrice());
                childfinprice = (tour.ChildPrice.RetreivePrice());
                babyfinprice = (tour.BabyPrice.RetreivePrice());
			}
			else
            {
                int deposit = int.Parse(_contentRep.GetContentById(1018).ContentText);
				adultfinprice = (tour.AdultPrice.RetreivePrice() * deposit) / 100;
				childfinprice = (tour.ChildPrice.RetreivePrice() * deposit) / 100;
				babyfinprice = (tour.BabyPrice.RetreivePrice() * deposit) / 100;
			}
            factor.OnePrice = ((adult * tour.AdultPrice.RetreivePrice()) + (child * tour.ChildPrice.RetreivePrice()) + (baby * tour.BabyPrice.RetreivePrice())).FixPrice();
            factor.FinPrice = ((adult * adultfinprice) + (child * childfinprice) + (baby *babyfinprice)).FixPrice();
			factor.Maliat = bool.Parse(GetCookie("FactorReq")) ? (factor.FinPrice.RetreivePrice() * maliatPrc).FixPrice() : "0";
			factor.TotalPrice = (factor.FinPrice.RetreivePrice() + factor.Maliat.RetreivePrice()).FixPrice();
			return factor;
        }

        public IActionResult CheckCapacity(int AdultCount, int ChildCount,int BabyCount) // an Action that remoted for check the field value validation (no need to post page)
        {
            int totalCount = AdultCount + ChildCount + BabyCount;
            if (totalCount <= _tourRep.GetTourById(int.Parse(GetCookie("CurTourId"))).Capacity)
            {
                return Json(true); // send true value
            }
            else return Json("تعداد بلیط درخواستی بیش از ظرفیت تور است"); //send error text
        }

		#region ManageCookies

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