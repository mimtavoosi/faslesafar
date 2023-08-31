using FasleSafar.Data.Repositories;
using FasleSafar.Data.Services;
using FasleSafar.Models;
using FasleSafar.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SmsSender;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.ServiceModel;
using System.Text;

namespace FasleSafar.Controllers
{
    public class HomeController : Controller
    {
		private IContentRep _contentRep;
        private ITourRep _tourRep;
        private IDestinationRep _destinationRep;
        private IReqTripRep _reqTripRep;
        private IUserRep _userRep;
        private IAttractionRep _attractionRep;
        private ITokenRep _tokenRep;
		private readonly BasicHttpBinding _binding;
		private readonly EndpointAddress _endpoint;
		public HomeController(IContentRep contentRep, ITourRep tourRep, IDestinationRep destinationRep, IReqTripRep reqTripRep,IUserRep userRep,IAttractionRep attractionRep,ITokenRep tokenRep)
        {
            _contentRep = contentRep;
            _tourRep = tourRep;
            _destinationRep = destinationRep;
            _reqTripRep = reqTripRep;
            _userRep = userRep;
            _attractionRep = attractionRep;
            _tokenRep = tokenRep;
			_binding = new BasicHttpBinding();
			_endpoint = new EndpointAddress("http://panel.payamakpardaz.com/smsSendWebService.asmx");
		}

        public IActionResult Index()
        {
            ViewBag.Ext = _contentRep.GetContentById(1020).ContentText;
            initSearchVals();
            return View();
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
			List<DateVM> dates = _tourRep.GetOpenTours().Select(d=> new DateVM()
            {
                DbDate = d.StartDate,
                ShowDate = DispDate(d.StartDate)
            }).ToList();
            dates = dates.DistinctBy(d=> d.DbDate).OrderByDescending(d=> d.DbDate).ToList();
            return dates;
		}

		private string DispDate(string startDate)
		{
            string[]arr=startDate.Split('/');
            return $"{arr[2]} {GetMonthName(arr[1])} {arr[0]}";  

		}

		public IActionResult ViewContent(int id)
        {
            return View(_contentRep.GetContentById(id));
        }
        public IActionResult ViewTour(int id)
        {
            var tour = _tourRep.GetTourById(id);
            TourVM tourVM = new TourVM()
            {
                AvgScore = tour.AvgScore,
                Capacity = tour.Capacity,
                DaysCount = tour.DaysCount,
                DestinationId = tour.DestinationId,
                EndDate = tour.EndDate,
                OpenState = tour.OpenState,
                Price = tour.Price.Value.FixPrice(),
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
            if(User.Identity.IsAuthenticated)
			{
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                ViewBag.Rate = _tourRep.ExistRating(userId, tourVM.TourId) ? "Exist" : "Not Exist";
            }
            ViewBag.Prices = _tourRep.GetHotelStaringsListOfTour(tour.TourId);
            ViewBag.DepositOn = (tourVM.IsLeasing) && (int.Parse(_contentRep.GetContentById(1018).ContentText) > 0) ? "on" : "off";
            return View(tourVM);
        }

		public IActionResult ViewDestination(int id)
		{
            ViewBag.Tours = _tourRep.GetOpenToursOfDestination(id).Take(3).ToList().Take(3).ToList().Select(t => new TourVM()
			{
				AvgScore = t.AvgScore,
				Capacity = t.Capacity,
				DaysCount = t.DaysCount,
				DestinationId = t.DestinationId,
				EndDate = t.EndDate,
				OpenState = t.OpenState,
				Price = t.Price.Value.FixPrice(),
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
			ViewBag.Attractions = _attractionRep.GetAttractionsOfDestination(id);
			return View(_destinationRep.GetDestinationById(id));
		}

		public IActionResult ViewAttraction(int id)
		{
			return View(_attractionRep.GetAttractionById(id));
		}

		public IActionResult ShowNewTours(int pageid = 1)
        {
			//_newToursPageNumber = pageid;
			initSearchVals();
			int skip = (pageid - 1) * 20;
            SetCookie("NewToursSkip", skip.ToString());
            int Count = _tourRep.GetOpenTours().Count();
            ViewBag.NewToursPageID = pageid;
            ViewBag.NewToursPageCount = Count / 20;
            ViewBag.ToursCount = Count;
            ViewBag.StartIndex = (Count > 0 ? ((pageid > 1) ? (20 * --pageid) + 1 : 1) : 0);
            ViewBag.EndIndex = (Count > 20) ? 20 : Count;
            List<TourVM> tourVMs = _tourRep.GetOpenToursForPages(skip).Select(t => new TourVM()
            {
                AvgScore = t.AvgScore,
                Capacity = t.Capacity,
                DaysCount = t.DaysCount,
                DestinationId = t.DestinationId,
                EndDate = t.EndDate,
                OpenState = t.OpenState,
                Price = t.Price.Value.FixPrice(),
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

		public IActionResult ShowIranTours(int pageid = 1)
		{
			//_iranToursPageNumber = pageid;
			initSearchVals();
			int skip = (pageid - 1) * 20;
			SetCookie("IranToursSkip",skip.ToString());
			int Count = _tourRep.GetIranTours().Count();
			ViewBag.IranToursPageID = pageid;
			ViewBag.IranToursPageCount = Count / 20;
			ViewBag.ToursCount = Count;
			ViewBag.StartIndex = (Count > 0 ? ((pageid > 1) ? (20 * --pageid) + 1 : 1) : 0);
			ViewBag.EndIndex = (Count > 20) ? 20 : Count;
			List<TourVM> tourVMs = _tourRep.GetIranToursForPages(skip).Select(t => new TourVM()
			{
				AvgScore = t.AvgScore,
				Capacity = t.Capacity,
				DaysCount = t.DaysCount,
				DestinationId = t.DestinationId,
				EndDate = t.EndDate,
				OpenState = t.OpenState,
				Price = t.Price.Value.FixPrice(),
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

		public IActionResult ShowWorldTours(int pageid = 1)
		{
			//_worldToursPageNumber = pageid;
			initSearchVals();
			int skip = (pageid - 1) * 20;
            SetCookie("WorldToursSkip", skip.ToString());
			int Count = _tourRep.GetWorldTours().Count();
			ViewBag.WorldToursPageID = pageid;
			ViewBag.WorldToursPageCount = Count / 20;
			ViewBag.ToursCount = Count;
			ViewBag.StartIndex = (Count > 0 ? ((pageid > 1) ? (20 * --pageid) + 1 : 1) : 0);
			ViewBag.EndIndex = (Count > 20) ? 20 : Count;
			List<TourVM> tourVMs = _tourRep.GetWorldToursForPages(skip).Select(t => new TourVM()
			{
				AvgScore = t.AvgScore,
				Capacity = t.Capacity,
				DaysCount = t.DaysCount,
				DestinationId = t.DestinationId,
				EndDate = t.EndDate,
				OpenState = t.OpenState,
				Price = t.Price.Value.FixPrice(),
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

		public IActionResult ShowFavoriteTours(int pageid = 1)
        {
			//_favoriteToursPageNumber = pageid;
			initSearchVals();
			int skip = (pageid - 1) * 20;
            SetCookie("FavoriteToursSkip", skip.ToString());
            int Count = _tourRep.GetFavoriteTours().Count();
            ViewBag.FavoriteToursPageID = pageid;
            ViewBag.FavoriteToursPageCount = Count / 20;
            ViewBag.ToursCount = Count;
            ViewBag.StartIndex = (Count > 0 ? ((pageid > 1) ? (20 * --pageid) + 1 : 1) : 0);
            ViewBag.EndIndex = (Count > 20) ? 20 : Count;
            List<TourVM> tourVMs = _tourRep.GetFavoriteToursForPages(skip).Select(t => new TourVM()
            {
                AvgScore = t.AvgScore,
                Capacity = t.Capacity,
                DaysCount = t.DaysCount,
                DestinationId = t.DestinationId,
                EndDate = t.EndDate,
                OpenState = t.OpenState,
                Price = t.Price.Value.FixPrice(),
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

        public IActionResult ShowDestinationTours(int id,int pageid = 1)
        {
			//_destinationToursPageNumber = pageid;
			initSearchVals();
			int skip = (pageid - 1) * 20;
            SetCookie("DestinationToursSkip", skip.ToString());
            SetCookie("CurDestId", id.ToString());
            int Count = _tourRep.GetOpenToursOfDestination(id).Count();
            ViewBag.DestinationToursPageID = pageid;
            ViewBag.DestinationToursPageCount = Count / 20;
            ViewBag.DestinationId = id;
            ViewBag.DestinationTitle = _destinationRep.GetDestinationById(id).DestinationName;
            ViewBag.ToursCount = Count;
            ViewBag.StartIndex = (Count > 0 ? ((pageid > 1) ? (20 * --pageid) + 1 : 1) : 0);
            ViewBag.EndIndex = (Count > 20) ? 20 : Count;
            List<TourVM> tourVMs = _tourRep.GetOpenToursOfDestinationForPages(id,skip).Select(t => new TourVM()
            {
                AvgScore = t.AvgScore,
                Capacity = t.Capacity,
                DaysCount = t.DaysCount,
                DestinationId = t.DestinationId,
                EndDate = t.EndDate,
                OpenState = t.OpenState,
                Price = t.Price.Value.FixPrice(),
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
        [HttpPost]
        public IActionResult SearchTours(string city,string type,string date,int dest,int days)
        {
            List<TourVM> searchlist = new List<TourVM>();
            bool checkcity=!string.IsNullOrEmpty(city);
            bool checktype=!string.IsNullOrEmpty(type);
            bool checkdate=!string.IsNullOrEmpty(date);
            bool checkdest = dest > 0;
            bool checkdays=days >0;
			initSearchVals();
			searchlist = _tourRep.GetOpenTours().Select(t => new TourVM()
            {
                AvgScore = t.AvgScore,
                Capacity = t.Capacity,
                DaysCount = t.DaysCount,
                DestinationId = t.DestinationId,
                EndDate = t.EndDate,
                OpenState = t.OpenState,
                Price = t.Price.Value.FixPrice(),
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
            if(checkcity)
            {
                SetCookie("SearchedCity",city);
                searchlist = searchlist.Where(t => t.TourTitle.Contains(city) || t.Destination.DestinationName.Contains(city)).ToList();
            }
            else SetCookie("SearchedCity", "");
            if (checktype)
            {
                SetCookie("SearchedType", type);
                searchlist = searchlist.Where(t => t.TourType == type).ToList();
            }
            else SetCookie("SearchedType", "");
            if (checkdate)
            {
                SetCookie("SearchedDate", date);
                searchlist = searchlist.Where(t => t.StartDate == date).ToList();
            }
            else SetCookie("SearchedDate", "");
            if (checkdest)
            {
                SetCookie("SearchedDest", dest.ToString());
                searchlist = searchlist.Where(t => t.DestinationId == dest).ToList();
            }
            else SetCookie("SearchedDest", "");
            if (checkdays)
            {
                SetCookie("SearchedDays", days.ToString());
                searchlist = searchlist.Where(t => t.DaysCount>= days).ToList();
            }
            else SetCookie("SearchedDays", "");
            int Count = searchlist.Count;
            ViewBag.ToursCount = Count;
            ViewBag.StartIndex = Count > 0 ? 1 :0;
            ViewBag.EndIndex = Count;
            return View(searchlist);
        }
        [HttpPost]
        public void Rating(int tourId, int rating)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            Tour tour = _tourRep.GetTourById(tourId);
            tour.TotalScore += rating;
            tour.ScoreCount += 1;
            tour.AvgScore = tour.TotalScore / tour.ScoreCount;
            tour.AvgScore = (float)Math.Round(tour.AvgScore,1);
            _tourRep.EditTour(tour);
            RatingHistory ratingHistory = new RatingHistory()
            {
                TourId = tourId,
                UserId = userId,
                Rate = rating
            };
            _tourRep.AddRating(ratingHistory);
        }
        private string GetMonthNumber(string v)
        {
            switch (v)
            {
                case "فروردین":
                default:
                    return "1";
                case "اردیبهشت":
                    return "2";
                case "خرداد":
                    return "3";
                case "تیر":
                    return "4";
                case "مرداد":
                    return "5";
                case "شهریور":
                    return "6";
                case "مهر":
                    return "7";
                case "آبان":
                    return "8";
                case "آذر":
                    return "9";
                case "دی":
                    return "10";
                case "بهمن":
                    return "11";
                case "اسفند":
                    return "12";
            }
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

		[Authorize]
        public IActionResult SendReqTrip()
		{
			return View();
		}

		[HttpPost] //Post Action For Send Data
		[ValidateAntiForgeryToken]
		public IActionResult SendReqTrip(SendReqTripVM reqTripVM)
		{
			if (!ModelState.IsValid) //If Input Data in form is invalid
			{
				return View(reqTripVM);
			}

            ReqTrip reqTrip = new ReqTrip()
            {
                UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                CreateDate = DateTime.Now.ToString("yyyy/MM/dd - HH:mm").ToShamsi(),
                DestinationName = reqTripVM.DestinationName,
                StartDate = reqTripVM.StartDate,
                EndDate = reqTripVM.EndDate,
                PassengersCount = reqTripVM.PassengersCount,
                ReqTripDescription = reqTripVM.ReqTripDescription,
                TransportType = reqTripVM.TransportType
			};
            _reqTripRep.AddReqTrip(reqTrip);
            SendReqTripNotice(reqTrip);
			return Redirect("/");
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }

        private decimal RetreivePrice(string finalPrice)
        {
            for (int i = 0; i < finalPrice.Length; i++)
            {
                if (finalPrice[i] == ',') finalPrice.Remove(i, 1);
            }
            return decimal.Parse(finalPrice);
        }

        [HttpPost]
        public async Task<string> UpdatePrice(string priceId)
        {
            if (priceId == null) return null;
            int id = int.Parse(priceId);
            if (_tourRep.GetHotelStaringById(id) == null) return null;
            return _tourRep.GetHotelStaringById(id).Price.Value.FixPrice() + " تومان";
        }

        #region Sortings

        [HttpPost]
        public async Task<List<TourVM>> SortTours(string sortType, string sortPage)
        {
            switch (sortPage)
            {
                case "1":
                default:
                    {
                        switch (sortType)
                        {
                            case "1":
                            default:
                                return _tourRep.GetOpenToursForPages(int.Parse(GetCookie("NewToursSkip"))).Select(t => new TourVM()
                                {
                                    AvgScore = t.AvgScore,
                                    Capacity = t.Capacity,
                                    DaysCount = t.DaysCount,
                                    DestinationId = t.DestinationId,
                                    EndDate = t.EndDate,
                                    OpenState = t.OpenState,
                                    Price = t.Price.Value.FixPrice(),
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

                                }).OrderBy(t => t.TourTitle).ToList();
                            case "2":
                                return _tourRep.GetOpenToursForPages(int.Parse(GetCookie("NewToursSkip"))).Select(t => new TourVM()
                                {
                                    AvgScore = t.AvgScore,
                                    Capacity = t.Capacity,
                                    DaysCount = t.DaysCount,
                                    DestinationId = t.DestinationId,
                                    EndDate = t.EndDate,
                                    OpenState = t.OpenState,
                                    Price = t.Price.Value.FixPrice(),
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

                                }).OrderByDescending(t => t.TourTitle).ToList();
                            case "3":
                                return _tourRep.GetOpenToursForPages(int.Parse(GetCookie("NewToursSkip"))).Select(t => new TourVM()
                                {
                                    AvgScore = t.AvgScore,
                                    Capacity = t.Capacity,
                                    DaysCount = t.DaysCount,
                                    DestinationId = t.DestinationId,
                                    EndDate = t.EndDate,
                                    OpenState = t.OpenState,
                                    Price = t.Price.Value.FixPrice(),
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

                                }).OrderBy(t => RetreivePrice(t.Price)).ToList();
                            case "4":
                                return _tourRep.GetOpenToursForPages(int.Parse(GetCookie("NewToursSkip"))).Select(t => new TourVM()
                                {
                                    AvgScore = t.AvgScore,
                                    Capacity = t.Capacity,
                                    DaysCount = t.DaysCount,
                                    DestinationId = t.DestinationId,
                                    EndDate = t.EndDate,
                                    OpenState = t.OpenState,
                                    Price = t.Price.Value.FixPrice(),
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

                                }).OrderByDescending(t => RetreivePrice(t.Price)).ToList();
                            case "5":
                                return _tourRep.GetOpenToursForPages(int.Parse(GetCookie("NewToursSkip"))).Select(t => new TourVM()
                                {
                                    AvgScore = t.AvgScore,
                                    Capacity = t.Capacity,
                                    DaysCount = t.DaysCount,
                                    DestinationId = t.DestinationId,
                                    EndDate = t.EndDate,
                                    OpenState = t.OpenState,
                                    Price = t.Price.Value.FixPrice(),
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

                                }).OrderByDescending(t => t.AvgScore).ToList();
                            case "6":
                                return _tourRep.GetOpenToursForPages(int.Parse(GetCookie("NewToursSkip"))).Select(t => new TourVM()
                                {
                                    AvgScore = t.AvgScore,
                                    Capacity = t.Capacity,
                                    DaysCount = t.DaysCount,
                                    DestinationId = t.DestinationId,
                                    EndDate = t.EndDate,
                                    OpenState = t.OpenState,
                                    Price = t.Price.Value.FixPrice(),
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

                                }).OrderBy(t => t.AvgScore).ToList();
                        }
                    }
                case "2":
                    {
                        switch (sortType)
                        {
                            case "1":
                            default:
                                return _tourRep.GetFavoriteToursForPages(int.Parse(GetCookie("FavoriteToursSkip"))).Select(t => new TourVM()
                                {
                                    AvgScore = t.AvgScore,
                                    Capacity = t.Capacity,
                                    DaysCount = t.DaysCount,
                                    DestinationId = t.DestinationId,
                                    EndDate = t.EndDate,
                                    OpenState = t.OpenState,
                                    Price = t.Price.Value.FixPrice(),
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

                                }).OrderBy(t => t.TourTitle).ToList();
                            case "2":
                                return _tourRep.GetFavoriteToursForPages(int.Parse(GetCookie("FavoriteToursSkip"))).Select(t => new TourVM()
                                {
                                    AvgScore = t.AvgScore,
                                    Capacity = t.Capacity,
                                    DaysCount = t.DaysCount,
                                    DestinationId = t.DestinationId,
                                    EndDate = t.EndDate,
                                    OpenState = t.OpenState,
                                    Price = t.Price.Value.FixPrice(),
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

                                }).OrderByDescending(t => t.TourTitle).ToList();
                            case "3":
                                return _tourRep.GetFavoriteToursForPages(int.Parse(GetCookie("FavoriteToursSkip"))).Select(t => new TourVM()
                                {
                                    AvgScore = t.AvgScore,
                                    Capacity = t.Capacity,
                                    DaysCount = t.DaysCount,
                                    DestinationId = t.DestinationId,
                                    EndDate = t.EndDate,
                                    OpenState = t.OpenState,
                                    Price = t.Price.Value.FixPrice(),
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

                                }).OrderBy(t => RetreivePrice(t.Price)).ToList();
                            case "4":
                                return _tourRep.GetFavoriteToursForPages(int.Parse(GetCookie("FavoriteToursSkip"))).Select(t => new TourVM()
                                {
                                    AvgScore = t.AvgScore,
                                    Capacity = t.Capacity,
                                    DaysCount = t.DaysCount,
                                    DestinationId = t.DestinationId,
                                    EndDate = t.EndDate,
                                    OpenState = t.OpenState,
                                    Price = t.Price.Value.FixPrice(),
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

                                }).OrderByDescending(t => RetreivePrice(t.Price)).ToList();
                            case "5":
                                return _tourRep.GetFavoriteToursForPages(int.Parse(GetCookie("FavoriteToursSkip"))).Select(t => new TourVM()
                                {
                                    AvgScore = t.AvgScore,
                                    Capacity = t.Capacity,
                                    DaysCount = t.DaysCount,
                                    DestinationId = t.DestinationId,
                                    EndDate = t.EndDate,
                                    OpenState = t.OpenState,
                                    Price = t.Price.Value.FixPrice(),
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

                                }).OrderByDescending(t => t.AvgScore).ToList();
                            case "6":
                                return _tourRep.GetFavoriteToursForPages(int.Parse(GetCookie("FavoriteToursSkip"))).Select(t => new TourVM()
                                {
                                    AvgScore = t.AvgScore,
                                    Capacity = t.Capacity,
                                    DaysCount = t.DaysCount,
                                    DestinationId = t.DestinationId,
                                    EndDate = t.EndDate,
                                    OpenState = t.OpenState,
                                    Price = t.Price.Value.FixPrice(),
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
                                }).OrderBy(t => t.AvgScore).ToList();
                        }
                    }
                case "3":
                    {
                        switch (sortType)
                        {
                            case "1":
                            default:
                                return _tourRep.GetOpenToursOfDestinationForPages(int.Parse(GetCookie("CurDestId")), int.Parse(GetCookie("DestinationToursSkip"))).Select(t => new TourVM()
                                {
                                    AvgScore = t.AvgScore,
                                    Capacity = t.Capacity,
                                    DaysCount = t.DaysCount,
                                    DestinationId = t.DestinationId,
                                    EndDate = t.EndDate,
                                    OpenState = t.OpenState,
                                    Price = t.Price.Value.FixPrice(),
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
                                }).OrderBy(t => t.TourTitle).ToList();
                            case "2":
                                return _tourRep.GetOpenToursOfDestinationForPages(int.Parse(GetCookie("CurDestId")), int.Parse(GetCookie("DestinationToursSkip"))).Select(t => new TourVM()
                                {
                                    AvgScore = t.AvgScore,
                                    Capacity = t.Capacity,
                                    DaysCount = t.DaysCount,
                                    DestinationId = t.DestinationId,
                                    EndDate = t.EndDate,
                                    OpenState = t.OpenState,
                                    Price = t.Price.Value.FixPrice(),
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
                                }).OrderByDescending(t => t.TourTitle).ToList();
                            case "3":
                                return _tourRep.GetOpenToursOfDestinationForPages(int.Parse(GetCookie("CurDestId")), int.Parse(GetCookie("DestinationToursSkip"))).Select(t => new TourVM()
                                {
                                    AvgScore = t.AvgScore,
                                    Capacity = t.Capacity,
                                    DaysCount = t.DaysCount,
                                    DestinationId = t.DestinationId,
                                    EndDate = t.EndDate,
                                    OpenState = t.OpenState,
                                    Price = t.Price.Value.FixPrice(),
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
                                }).OrderBy(t => RetreivePrice(t.Price)).ToList();
                            case "4":
                                return _tourRep.GetOpenToursOfDestinationForPages(int.Parse(GetCookie("CurDestId")), int.Parse(GetCookie("DestinationToursSkip"))).Select(t => new TourVM()
                                {
                                    AvgScore = t.AvgScore,
                                    Capacity = t.Capacity,
                                    DaysCount = t.DaysCount,
                                    DestinationId = t.DestinationId,
                                    EndDate = t.EndDate,
                                    OpenState = t.OpenState,
                                    Price = t.Price.Value.FixPrice(),
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
                                }).OrderByDescending(t => RetreivePrice(t.Price)).ToList();
                            case "5":
                                return _tourRep.GetOpenToursOfDestinationForPages(int.Parse(GetCookie("CurDestId")), int.Parse(GetCookie("DestinationToursSkip"))).Select(t => new TourVM()
                                {
                                    AvgScore = t.AvgScore,
                                    Capacity = t.Capacity,
                                    DaysCount = t.DaysCount,
                                    DestinationId = t.DestinationId,
                                    EndDate = t.EndDate,
                                    OpenState = t.OpenState,
                                    Price = t.Price.Value.FixPrice(),
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

                                }).OrderByDescending(t => t.AvgScore).ToList();
                            case "6":
                                return _tourRep.GetOpenToursOfDestinationForPages(int.Parse(GetCookie("CurDestId")), int.Parse(GetCookie("DestinationToursSkip"))).Select(t => new TourVM()
                                {
                                    AvgScore = t.AvgScore,
                                    Capacity = t.Capacity,
                                    DaysCount = t.DaysCount,
                                    DestinationId = t.DestinationId,
                                    EndDate = t.EndDate,
                                    OpenState = t.OpenState,
                                    Price = t.Price.Value.FixPrice(),
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
                                }).OrderBy(t => t.AvgScore).ToList();
                        }
                    }
                case "4":
                    {
                        int userid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                        switch (sortType)
                        {
                            case "1":
                            default:
                                return _tourRep.GetToursOfUserForPages(userid, int.Parse(GetCookie("UserToursSkip"))).Select(t => new TourVM()
                                {
                                    AvgScore = t.AvgScore,
                                    Capacity = t.Capacity,
                                    DaysCount = t.DaysCount,
                                    DestinationId = t.DestinationId,
                                    EndDate = t.EndDate,
                                    OpenState = t.OpenState,
                                    Price = t.Price.Value.FixPrice(),
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

                                }).OrderBy(t => t.TourTitle).ToList();
                            case "2":
                                return _tourRep.GetToursOfUserForPages(userid, int.Parse(GetCookie("UserToursSkip"))).Select(t => new TourVM()
                                {
                                    AvgScore = t.AvgScore,
                                    Capacity = t.Capacity,
                                    DaysCount = t.DaysCount,
                                    DestinationId = t.DestinationId,
                                    EndDate = t.EndDate,
                                    OpenState = t.OpenState,
                                    Price = t.Price.Value.FixPrice(),
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

                                }).OrderByDescending(t => t.TourTitle).ToList();
                            case "3":
                                return _tourRep.GetToursOfUserForPages(userid, int.Parse(GetCookie("UserToursSkip"))).Select(t => new TourVM()
                                {
                                    AvgScore = t.AvgScore,
                                    Capacity = t.Capacity,
                                    DaysCount = t.DaysCount,
                                    DestinationId = t.DestinationId,
                                    EndDate = t.EndDate,
                                    OpenState = t.OpenState,
                                    Price = t.Price.Value.FixPrice(),
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

                                }).OrderBy(t => RetreivePrice(t.Price)).ToList();
                            case "4":
                                return _tourRep.GetToursOfUserForPages(userid, int.Parse(GetCookie("UserToursSkip"))).Select(t => new TourVM()
                                {
                                    AvgScore = t.AvgScore,
                                    Capacity = t.Capacity,
                                    DaysCount = t.DaysCount,
                                    DestinationId = t.DestinationId,
                                    EndDate = t.EndDate,
                                    OpenState = t.OpenState,
                                    Price = t.Price.Value.FixPrice(),
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

                                }).OrderByDescending(t => RetreivePrice(t.Price)).ToList();
                            case "5":
                                return _tourRep.GetToursOfUserForPages(userid, int.Parse(GetCookie("UserToursSkip"))).Select(t => new TourVM()
                                {
                                    AvgScore = t.AvgScore,
                                    Capacity = t.Capacity,
                                    DaysCount = t.DaysCount,
                                    DestinationId = t.DestinationId,
                                    EndDate = t.EndDate,
                                    OpenState = t.OpenState,
                                    Price = t.Price.Value.FixPrice(),
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

                                }).OrderByDescending(t => t.AvgScore).ToList();
                            case "6":
                                return _tourRep.GetToursOfUserForPages(userid, int.Parse(GetCookie("UserToursSkip"))).Select(t => new TourVM()
                                {
                                    AvgScore = t.AvgScore,
                                    Capacity = t.Capacity,
                                    DaysCount = t.DaysCount,
                                    DestinationId = t.DestinationId,
                                    EndDate = t.EndDate,
                                    OpenState = t.OpenState,
                                    Price = t.Price.Value.FixPrice(),
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
                                }).OrderBy(t => t.AvgScore).ToList();
                        }
                    }
                case "5":
                    {

                        switch (sortType)
                        {
                            case "1":
                            default:
                                return FoundTours().OrderBy(t => t.TourTitle).ToList();
                            case "2":
                                return FoundTours().OrderByDescending(t => t.TourTitle).ToList();
                            case "3":
                                return FoundTours().OrderBy(t => RetreivePrice(t.Price)).ToList();
                            case "4":
                                return FoundTours().OrderByDescending(t => RetreivePrice(t.Price)).ToList();
                            case "5":
                                return FoundTours().OrderByDescending(t => t.AvgScore).ToList();
                            case "6":
                                return FoundTours().OrderBy(t => t.AvgScore).ToList();
                        }
                    }
				case "6":
					{
						switch (sortType)
						{
							case "1":
							default:
								return _tourRep.GetIranToursForPages(int.Parse(GetCookie("IranToursSkip"))).Select(t => new TourVM()
								{
									AvgScore = t.AvgScore,
									Capacity = t.Capacity,
									DaysCount = t.DaysCount,
									DestinationId = t.DestinationId,
									EndDate = t.EndDate,
									OpenState = t.OpenState,
									Price = t.Price.Value.FixPrice(),
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

                                }).OrderBy(t => t.TourTitle).ToList();
							case "2":
								return _tourRep.GetIranToursForPages(int.Parse(GetCookie("IranToursSkip"))).Select(t => new TourVM()
								{
									AvgScore = t.AvgScore,
									Capacity = t.Capacity,
									DaysCount = t.DaysCount,
									DestinationId = t.DestinationId,
									EndDate = t.EndDate,
									OpenState = t.OpenState,
									Price = t.Price.Value.FixPrice(),
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

                                }).OrderByDescending(t => t.TourTitle).ToList();
							case "3":
								return _tourRep.GetIranToursForPages(int.Parse(GetCookie("IranToursSkip"))).Select(t => new TourVM()
								{
									AvgScore = t.AvgScore,
									Capacity = t.Capacity,
									DaysCount = t.DaysCount,
									DestinationId = t.DestinationId,
									EndDate = t.EndDate,
									OpenState = t.OpenState,
									Price = t.Price.Value.FixPrice(),
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

                                }).OrderBy(t => RetreivePrice(t.Price)).ToList();
							case "4":
								return _tourRep.GetIranToursForPages(int.Parse(GetCookie("IranToursSkip"))).Select(t => new TourVM()
								{
									AvgScore = t.AvgScore,
									Capacity = t.Capacity,
									DaysCount = t.DaysCount,
									DestinationId = t.DestinationId,
									EndDate = t.EndDate,
									OpenState = t.OpenState,
									Price = t.Price.Value.FixPrice(),
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

                                }).OrderByDescending(t => RetreivePrice(t.Price)).ToList();
							case "5":
								return _tourRep.GetIranToursForPages(int.Parse(GetCookie("IranToursSkip"))).Select(t => new TourVM()
								{
									AvgScore = t.AvgScore,
									Capacity = t.Capacity,
									DaysCount = t.DaysCount,
									DestinationId = t.DestinationId,
									EndDate = t.EndDate,
									OpenState = t.OpenState,
									Price = t.Price.Value.FixPrice(),
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

                                }).OrderByDescending(t => t.AvgScore).ToList();
							case "6":
								return _tourRep.GetIranToursForPages(int.Parse(GetCookie("IranToursSkip"))).Select(t => new TourVM()
								{
									AvgScore = t.AvgScore,
									Capacity = t.Capacity,
									DaysCount = t.DaysCount,
									DestinationId = t.DestinationId,
									EndDate = t.EndDate,
									OpenState = t.OpenState,
									Price = t.Price.Value.FixPrice(),
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

                                }).OrderBy(t => t.AvgScore).ToList();
						}
					}
				case "7":
					{
						switch (sortType)
						{
							case "1":
							default:
								return _tourRep.GetWorldToursForPages(int.Parse(GetCookie("WorldToursSkip"))).Select(t => new TourVM()
								{
									AvgScore = t.AvgScore,
									Capacity = t.Capacity,
									DaysCount = t.DaysCount,
									DestinationId = t.DestinationId,
									EndDate = t.EndDate,
									OpenState = t.OpenState,
									Price = t.Price.Value.FixPrice(),
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

                                }).OrderBy(t => t.TourTitle).ToList();
							case "2":
								return _tourRep.GetWorldToursForPages(int.Parse(GetCookie("WorldToursSkip"))).Select(t => new TourVM()
								{
									AvgScore = t.AvgScore,
									Capacity = t.Capacity,
									DaysCount = t.DaysCount,
									DestinationId = t.DestinationId,
									EndDate = t.EndDate,
									OpenState = t.OpenState,
									Price = t.Price.Value.FixPrice(),
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

                                }).OrderByDescending(t => t.TourTitle).ToList();
							case "3":
								return _tourRep.GetWorldToursForPages(int.Parse(GetCookie("WorldToursSkip"))).Select(t => new TourVM()
								{
									AvgScore = t.AvgScore,
									Capacity = t.Capacity,
									DaysCount = t.DaysCount,
									DestinationId = t.DestinationId,
									EndDate = t.EndDate,
									OpenState = t.OpenState,
									Price = t.Price.Value.FixPrice(),
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

                                }).OrderBy(t => RetreivePrice(t.Price)).ToList();
							case "4":
								return _tourRep.GetWorldToursForPages(int.Parse(GetCookie("WorldToursSkip"))).Select(t => new TourVM()
								{
									AvgScore = t.AvgScore,
									Capacity = t.Capacity,
									DaysCount = t.DaysCount,
									DestinationId = t.DestinationId,
									EndDate = t.EndDate,
									OpenState = t.OpenState,
									Price = t.Price.Value.FixPrice(),
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

                                }).OrderByDescending(t => RetreivePrice(t.Price)).ToList();
							case "5":
								return _tourRep.GetWorldToursForPages(int.Parse(GetCookie("WorldToursSkip"))).Select(t => new TourVM()
								{
									AvgScore = t.AvgScore,
									Capacity = t.Capacity,
									DaysCount = t.DaysCount,
									DestinationId = t.DestinationId,
									EndDate = t.EndDate,
									OpenState = t.OpenState,
									Price = t.Price.Value.FixPrice(),
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

                                }).OrderByDescending(t => t.AvgScore).ToList();
							case "6":
								return _tourRep.GetWorldToursForPages(int.Parse(GetCookie("WorldToursSkip"))).Select(t => new TourVM()
								{
									AvgScore = t.AvgScore,
									Capacity = t.Capacity,
									DaysCount = t.DaysCount,
									DestinationId = t.DestinationId,
									EndDate = t.EndDate,
									OpenState = t.OpenState,
									Price = t.Price.Value.FixPrice(),
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

                                }).OrderBy(t => t.AvgScore).ToList();
						}
					}
			}
        }

        #endregion

        public List<TourVM> FoundTours()
        {
            var searchlist = _tourRep.GetOpenTours().Select(t => new TourVM()
            {
                AvgScore = t.AvgScore,
                Capacity = t.Capacity,
                DaysCount = t.DaysCount,
                DestinationId = t.DestinationId,
                EndDate = t.EndDate,
                OpenState = t.OpenState,
                Price = t.Price.Value.FixPrice(),
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
            if (GetCookie("SearchedCity") != "") searchlist = searchlist.Where(t => t.TourTitle.Contains(GetCookie("SearchedCity")) || t.Destination.DestinationName.Contains(GetCookie("SearchedCity"))).ToList();
            if (GetCookie("SearchedType") != "") searchlist = searchlist.Where(t => t.TourType == GetCookie("SearchedType")).ToList();
            if (GetCookie("SearchedDate") != "") searchlist = searchlist.Where(t => t.StartDate == GetCookie("SearchedDate")).ToList();
            if (GetCookie("SearchedDest") != "") searchlist = searchlist.Where(t => t.DestinationId == int.Parse(GetCookie("SearchedDest"))).ToList();
            if (GetCookie("SearchedDays") != "") searchlist = searchlist.Where(t => t.DaysCount >= int.Parse(GetCookie("SearchedDays"))).ToList();
            return searchlist;
        }

		#region ManageCookies

		//public void SetCookie(string key, string value, bool isPersistant = false)
		//{
		//    CookieOptions options = new CookieOptions() { IsEssential = true, HttpOnly = true };
		//    if (isPersistant) options.Expires = DateTime.Now.AddMinutes(20);
		//    Response.Cookies.Delete(key);
		//    Response.Cookies.Append(key, value, options);
		//}

		//public string GetCookie(string key)
		//{
		//    return Request.Cookies[key].ToString();
		//}

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

		#region SendMessage

		private async void SendReqTripNotice(ReqTrip reqTrip)
		{
            var user = _userRep.GetUserById(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));
			string message = MakeReqTripMessage(user.FullName, reqTrip);
			bool sent = await SendMessage(user.MobileNumber, message);
		}

		private string MakeReqTripMessage(string fullName, ReqTrip req)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine($"{fullName} عزیز!");
			sb.AppendLine($"درخواست سفر {req.TransportType} شما به مقصد {req.DestinationName}");
			sb.AppendLine($"از تاریخ {req.StartDate}");
			sb.AppendLine($"تا تاریخ {req.EndDate}");
			sb.AppendLine($"در تاریخ {req.CreateDate} ثبت شد");
			sb.AppendLine($"بزودی با شما تماس گرفته خواهد شد.");
			sb.AppendLine($"باتشکر از اعتماد شما");
			sb.AppendLine("آژانس مسافرتی فصل سفر اصفهان");
			return sb.ToString();
		}

		public async Task<bool> SendMessage(string mobileNumber, string messageText)
		{
			bool send = false;
			try
			{
				using (var client = new smsSendWebServiceSoapClient(_binding, _endpoint))
				{
					var response = await client.SendSingleSmsAsync("m.dorri", "7784914z", "panel.raysansms", messageText, mobileNumber, "30006403868611", SmsMode.SaveInSim);
					var res = response.Body.SendSingleSmsResult;
					send = response.Body.SendSingleSmsResult > 0;

				}
			}
			catch (Exception)
			{
				send = false;
			}
			return send;
		}

		#endregion
	}
}