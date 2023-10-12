using FasleSafar.Data.Repositories;
using FasleSafar.Models;
using FasleSafar.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ServiceModel;
using System.Text;
using SmsSender;
using System.IO;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Drawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FasleSafar.Controllers
{
	[Authorize]
	public class AdminController : Controller
	{
		private ITourRep _tourRep;
		private IOrderRep _orderRep;
		private IUserRep _userRep;
		private IDestinationRep _destinationRep;
		private IContentRep _contentRep;
		private IReqTripRep _reqTripRep;
		private IMessageRep _messageRep;
		private IAttractionRep _attractionRep;
		private IPassengerRep _passengerRep;
		private ITokenRep _tokenRep;
		private readonly BasicHttpBinding _binding;
		private readonly EndpointAddress _endpoint;

		public AdminController(ITourRep tourRep, IOrderRep orderRep, IUserRep userRep, IDestinationRep destinationRep, IContentRep contentRep, IReqTripRep reqTripRep, IMessageRep messageRep, IAttractionRep attractionRep, IPassengerRep passengerRep, ITokenRep tokenRep)
		{
			_tourRep = tourRep;
			_orderRep = orderRep;
			_userRep = userRep;
			_destinationRep = destinationRep;
			_contentRep = contentRep;
			_reqTripRep = reqTripRep;
			_messageRep = messageRep;
			_attractionRep = attractionRep;
			_passengerRep = passengerRep;
			_tokenRep = tokenRep;
			_binding = new BasicHttpBinding();
			_endpoint = new EndpointAddress("http://panel.payamakpardaz.com/smsSendWebService.asmx");
		}

		#region Search

		public JsonResult SearchRecords(string searchtext, int searchtype)
		{
			JsonResult jsonResult;
			switch (searchtype)
			{
				case 1:
				default:
					{
						if (!string.IsNullOrEmpty(searchtext))
						{
							var Searchlist = _destinationRep.GetAllDestinations().Where(d => (!string.IsNullOrEmpty(d.DestinationName.ToString()) && d.DestinationName.ToString().Contains(searchtext))
							).ToList();
							jsonResult = Json(Searchlist);
						}
						else
						{
							int skip = (int.Parse(GetCookie("DestinationsPageNumber")) - 1) * 20;
							int Count = _destinationRep.GetAllDestinations().Count();
							ViewBag.DestinationsPageID = GetCookie("DestinationsPageNumber");
							ViewBag.DestinationsPageCount = Count / 20;
							jsonResult = Json(_destinationRep.GetDestinationsForPages(skip));
						}
					}
					break;
				case 2:
					{
						if (!string.IsNullOrEmpty(searchtext))
						{
							var Searchlist = _tourRep.GetAllTours().Where(t => (!string.IsNullOrEmpty(t.TourTitle.ToString()) && t.TourTitle.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(t.TourType.ToString()) && t.TourType.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(t.TransportType.ToString()) && t.TransportType.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(t.Destination.DestinationName.ToString()) && t.Destination.DestinationName.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(t.Capacity.ToString()) && t.Capacity.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(t.DaysCount.ToString()) && t.DaysCount.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(t.StartDate.ToString()) && t.StartDate.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(t.EndDate.ToString()) && t.EndDate.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(t.Price.ToString()) && t.Price.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(t.Vehicle.ToString()) && t.Vehicle.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(t.AvgScore.ToString()) && t.AvgScore.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(t.TotalScore.ToString()) && t.TotalScore.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(t.ScoreCount.ToString()) && t.ScoreCount.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(t.OpenState.ToString()) && t.OpenState.ToString().Contains(searchtext))
							).ToList();
							List<TourVM> tourVMs = Searchlist.Select(t => new TourVM()
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
								IsLeasing = t.IsLeasing,
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
							jsonResult = Json(tourVMs);
						}
						else
						{
							int skip = (int.Parse(GetCookie("ToursPageNumber")) - 1) * 20;
							int Count = _tourRep.GetAllTours().Count();
							ViewBag.ToursPageID = GetCookie("ToursPageNumber");
							ViewBag.ToursPageCount = Count / 20;
							List<TourVM> tourVMs = _tourRep.GetToursForPages(skip).Select(t => new TourVM()
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
								IsLeasing = t.IsLeasing,
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
							jsonResult = Json(tourVMs);
						}
					}
					break;
				case 3:
					{
						if (!string.IsNullOrEmpty(searchtext))
						{
							var Searchlist = _tourRep.GetToursOfDestination(int.Parse(GetCookie("CurDestId"))).Where(t => (!string.IsNullOrEmpty(t.TourTitle.ToString()) && t.TourTitle.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(t.TourType.ToString()) && t.TourType.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(t.TransportType.ToString()) && t.TransportType.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(t.Destination.DestinationName.ToString()) && t.Destination.DestinationName.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(t.Capacity.ToString()) && t.Capacity.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(t.DaysCount.ToString()) && t.DaysCount.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(t.StartDate.ToString()) && t.StartDate.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(t.EndDate.ToString()) && t.EndDate.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(t.Price.ToString()) && t.Price.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(t.Vehicle.ToString()) && t.Vehicle.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(t.AvgScore.ToString()) && t.AvgScore.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(t.TotalScore.ToString()) && t.TotalScore.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(t.ScoreCount.ToString()) && t.ScoreCount.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(t.OpenState.ToString()) && t.OpenState.ToString().Contains(searchtext))
							).ToList();
							List<TourVM> tourVMs = Searchlist.Select(t => new TourVM()
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
								IsLeasing = t.IsLeasing,
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
							jsonResult = Json(tourVMs);
						}
						else
						{
							int skip = (int.Parse("DestinationToursPageNumber") - 1) * 20;
							int Count = _tourRep.GetToursOfDestination(int.Parse(GetCookie("CurDestId"))).Count();
							ViewBag.DestinationToursPageID = GetCookie("DestinationToursPageNumber");
							ViewBag.DestinationToursPageCount = Count / 20;
							List<TourVM> tourVMs = _tourRep.GetToursOfDestinationForPages(int.Parse(GetCookie("CurDestId")), skip).Select(t => new TourVM()
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
								IsLeasing = t.IsLeasing,
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
							jsonResult = Json(tourVMs);
						}
					}
					break;
				case 4:
					{
						if (!string.IsNullOrEmpty(searchtext))
						{
							var Searchlist = _userRep.GetAllUsers().Where(u => (!string.IsNullOrEmpty(u.FullName.ToString()) && u.FullName.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(u.Email.ToString()) && u.Email.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(u.MobileNumber.ToString()) && u.MobileNumber.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(u.Password.ToString()) && u.Password.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(u.IsAdmin.ToString()) && u.IsAdmin.ToString().Contains(searchtext))
							).ToList();
							jsonResult = Json(Searchlist);
						}
						else
						{
							int skip = (int.Parse(GetCookie("UsersPageNumber")) - 1) * 20;
							int Count = _userRep.GetAllUsers().Count();
							ViewBag.UsersPageID = GetCookie("UsersPageNumber");
							ViewBag.UsersPageCount = Count / 20;
							jsonResult = Json(_userRep.GetUsersForPages(skip));
						}
					}
					break;
				case 5:
					{
						if (!string.IsNullOrEmpty(searchtext))
						{
							var Searchlist = _userRep.GetUsersOfTour(int.Parse(GetCookie("CurTourId"))).Where(u => (!string.IsNullOrEmpty(u.FullName.ToString()) && u.FullName.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(u.Email.ToString()) && u.Email.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(u.MobileNumber.ToString()) && u.MobileNumber.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(u.Password.ToString()) && u.Password.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(u.IsAdmin.ToString()) && u.IsAdmin.ToString().Contains(searchtext))
							).ToList();
							jsonResult = Json(Searchlist);
						}
						else
						{
							int skip = (int.Parse(GetCookie("UsersofTourPageNumber")) - 1) * 20;
							int Count = _userRep.GetUsersOfTour(int.Parse(GetCookie("CurTourId"))).Count();
							ViewBag.TourUsersPageID = GetCookie("UsersofTourPageNumber");
							ViewBag.TourUsersPageCount = Count / 20;
							jsonResult = Json(_userRep.GetUsersOfTourForPages(int.Parse(GetCookie("CurTourId")), skip));
						}
					}
					break;
				case 6:
					{
						if (!string.IsNullOrEmpty(searchtext))
						{
							var Searchlist = _tourRep.GetToursOfUser(int.Parse(GetCookie("CurUserId"))).Where(t => (!string.IsNullOrEmpty(t.TourTitle.ToString()) && t.TourTitle.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(t.TourType.ToString()) && t.TourType.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(t.TransportType.ToString()) && t.TransportType.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(t.Destination.DestinationName.ToString()) && t.Destination.DestinationName.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(t.Capacity.ToString()) && t.Capacity.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(t.DaysCount.ToString()) && t.DaysCount.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(t.StartDate.ToString()) && t.StartDate.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(t.EndDate.ToString()) && t.EndDate.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(t.Price.ToString()) && t.Price.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(t.Vehicle.ToString()) && t.Vehicle.ToString().Contains(searchtext))
							 || (!string.IsNullOrEmpty(t.AvgScore.ToString()) && t.AvgScore.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(t.TotalScore.ToString()) && t.TotalScore.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(t.ScoreCount.ToString()) && t.ScoreCount.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(t.OpenState.ToString()) && t.OpenState.ToString().Contains(searchtext))
							).ToList();
							List<TourVM> tourVMs = Searchlist.Select(t => new TourVM()
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
								IsLeasing = t.IsLeasing,
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
							jsonResult = Json(tourVMs);
						}
						else
						{
							int skip = (int.Parse(GetCookie("ToursofUserPageNumber")) - 1) * 20;
							int Count = _tourRep.GetToursOfUser(int.Parse(GetCookie("CurUserId"))).Count();
							ViewBag.UserToursPageID = GetCookie("ToursofUserPageNumber");
							ViewBag.UserToursPageCount = Count / 20;
							List<TourVM> tourVMs = _tourRep.GetToursOfUserForPages(int.Parse(GetCookie("CurUserId")), skip).Select(t => new TourVM()
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
								IsLeasing = t.IsLeasing,
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
							jsonResult = Json(tourVMs);
						}
					}
					break;
				case 7:
					{
						if (!string.IsNullOrEmpty(searchtext))
						{
							var Searchlist = _orderRep.GetAllOrders().Where(od => (!string.IsNullOrEmpty(od.Tour.TourTitle.ToString()) && od.Tour.TourTitle.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(od.Price.ToString()) && od.Price.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(od.OrderId.ToString()) && od.OrderId.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(od.User.FullName.ToString()) && od.User.FullName.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(od.CreateDate.ToString()) && od.CreateDate.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(od.IsFinaly.ToString()) && od.IsFinaly.ToString().Contains(searchtext))
							).ToList();
							jsonResult = Json(Searchlist);
						}
						else
						{
							int skip = (int.Parse(GetCookie("OrdersPageNumber")) - 1) * 20;
							int Count = _orderRep.GetAllOrders().Count();
							ViewBag.OrdersPageID = GetCookie("OrdersPageNumber");
							ViewBag.OrdersPageCount = Count / 20;
							jsonResult = Json(_orderRep.GetOrdersForPages(skip));
						}
					}
					break;
				case 8:
					{
						if (!string.IsNullOrEmpty(searchtext))
						{
							var Searchlist = _orderRep.GetOrdersByUserId(int.Parse(GetCookie("CurUserId"))).Where(od => (!string.IsNullOrEmpty(od.Tour.TourTitle.ToString()) && od.Tour.TourTitle.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(od.Price.ToString()) && od.Price.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(od.OrderId.ToString()) && od.OrderId.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(od.User.FullName.ToString()) && od.User.FullName.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(od.CreateDate.ToString()) && od.CreateDate.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(od.IsFinaly.ToString()) && od.IsFinaly.ToString().Contains(searchtext))).ToList();
							jsonResult = Json(Searchlist);
						}
						else
						{
							int skip = (int.Parse(GetCookie("OrdersofUserPageNumber")) - 1) * 20;
							int Count = _orderRep.GetOrdersByUserId(int.Parse(GetCookie("CurUserId"))).Count();
							ViewBag.UserOrdersPageID = GetCookie("OrdersofUserPageNumber");
							ViewBag.UserOrdersPageCount = Count / 20;
							jsonResult = Json(_orderRep.GetOrdersOfUserForPages(int.Parse(GetCookie("CurUserId")), skip));
						}
					}
					break;
				case 9:
					{
						if (!string.IsNullOrEmpty(searchtext))
						{
							var Searchlist = _reqTripRep.GetAllReqTrips().Where(od => (!string.IsNullOrEmpty(od.User.FullName.ToString()) && od.User.FullName.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(od.DestinationName.ToString()) && od.DestinationName.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(od.StartDate.ToString()) && od.StartDate.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(od.EndDate.ToString()) && od.EndDate.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(od.PassengersCount.ToString()) && od.PassengersCount.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(od.TransportType.ToString()) && od.TransportType.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(od.CreateDate.ToString()) && od.CreateDate.ToString().Contains(searchtext))).ToList();
							jsonResult = Json(Searchlist);
						}
						else
						{
							int skip = (int.Parse(GetCookie("ReqTripsPageNumber")) - 1) * 20;
							int Count = _reqTripRep.GetAllReqTrips().Count();
							ViewBag.ReqTripsPageID = GetCookie("ReqTripsPageNumber");
							ViewBag.ReqTripsPageCount = Count / 20;
							jsonResult = Json(_reqTripRep.GetReqTripsForPages(skip));
						}
					}
					break;
				case 10:
					{
						if (!string.IsNullOrEmpty(searchtext))
						{
							var Searchlist = _reqTripRep.GetReqTripsByUserId(int.Parse(GetCookie("CurUserId"))).Where(od => (!string.IsNullOrEmpty(od.User.FullName.ToString()) && od.User.FullName.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(od.DestinationName.ToString()) && od.DestinationName.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(od.StartDate.ToString()) && od.StartDate.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(od.EndDate.ToString()) && od.EndDate.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(od.PassengersCount.ToString()) && od.PassengersCount.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(od.TransportType.ToString()) && od.TransportType.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(od.CreateDate.ToString()) && od.CreateDate.ToString().Contains(searchtext))).ToList();
							jsonResult = Json(Searchlist);
						}
						else
						{
							int skip = (int.Parse(GetCookie("ReqTripsofUserPageNumber")) - 1) * 20;
							int Count = _reqTripRep.GetReqTripsByUserId(int.Parse(GetCookie("CurUserId"))).Count();
							ViewBag.UserReqTripsPageID = GetCookie("ReqTripsofUserPageNumber");
							ViewBag.UserReqTripsPageCount = Count / 20;
							jsonResult = Json(_reqTripRep.GetReqTripsOfUserForPages(int.Parse(GetCookie("CurUserId")), skip));
						}
					}
					break;
				case 11:
					{
						if (!string.IsNullOrEmpty(searchtext))
						{
							var Searchlist = _messageRep.GetAllMessages().Where(m => (!string.IsNullOrEmpty(m.User.FullName.ToString()) && m.User.FullName.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(m.User.MobileNumber.ToString()) && m.User.MobileNumber.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(m.MessageText.ToString()) && m.MessageText.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(m.SentDate.ToString()) && m.SentDate.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(m.SentState.ToString()) && m.SentState.ToString().Contains(searchtext))
							).ToList();
							jsonResult = Json(Searchlist);
						}
						else
						{
							int skip = (int.Parse(GetCookie("MessagesPageNumber")) - 1) * 20;
							int Count = _messageRep.GetAllMessages().Count();
							ViewBag.MessagesPageID = GetCookie("MessagesPageNumber");
							ViewBag.MessagesPageCount = Count / 20;
							jsonResult = Json(_messageRep.GetMessagesForPages(skip));
						}
					}
					break;
				case 12:
					{
						if (!string.IsNullOrEmpty(searchtext))
						{
							var Searchlist = _messageRep.GetMessagesByUserId(int.Parse(GetCookie("CurUserId"))).Where(m => (!string.IsNullOrEmpty(m.User.FullName.ToString()) && m.User.FullName.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(m.User.MobileNumber.ToString()) && m.User.MobileNumber.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(m.MessageText.ToString()) && m.MessageText.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(m.SentDate.ToString()) && m.SentDate.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(m.SentState.ToString()) && m.SentState.ToString().Contains(searchtext))
							).ToList();
							jsonResult = Json(Searchlist);
						}
						else
						{
							int skip = (int.Parse(GetCookie("MessagesofUserPageNumber")) - 1) * 20;
							int Count = _messageRep.GetMessagesByUserId(int.Parse(GetCookie("CurUserId"))).Count();
							ViewBag.UserMessagesPageID = GetCookie("MessagesofUserPageNumber");
							ViewBag.UserMessagesPageCount = Count / 20;
							jsonResult = Json(_messageRep.GetMessagesOfUserForPages(int.Parse(GetCookie("CurUserId")), skip));
						}
					}
					break;
				case 13:
					{
						if (!string.IsNullOrEmpty(searchtext))
						{
							var Searchlist = _attractionRep.GetAllAttractions().Where(d => (!string.IsNullOrEmpty(d.AttractionName.ToString()) && d.AttractionName.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(d.Destination.DestinationName.ToString()) && d.Destination.DestinationName.ToString().Contains(searchtext))
							).ToList();
							jsonResult = Json(Searchlist);
						}
						else
						{
							int skip = (int.Parse(GetCookie("AttractionsPageNumber")) - 1) * 20;
							int Count = _attractionRep.GetAllAttractions().Count();
							ViewBag.AttractionsPageID = GetCookie("AttractionsPageNumber");
							ViewBag.AttractionsPageCount = Count / 20;
							jsonResult = Json(_attractionRep.GetAttractionsForPages(skip));
						}
					}
					break;
				case 14:
					{
						if (!string.IsNullOrEmpty(searchtext))
						{
							var Searchlist = _attractionRep.GetAttractionsOfDestination(int.Parse(GetCookie("CurDestId"))).Where(d => (!string.IsNullOrEmpty(d.AttractionName.ToString()) && d.AttractionName.ToString().Contains(searchtext))
							|| (!string.IsNullOrEmpty(d.Destination.DestinationName.ToString()) && d.Destination.DestinationName.ToString().Contains(searchtext))
							).ToList();
							jsonResult = Json(Searchlist);
						}
						else
						{
							int skip = (int.Parse(GetCookie("DestinationAttractionsPageNumber")) - 1) * 20;
							int Count = _attractionRep.GetAttractionsOfDestination(int.Parse(GetCookie("CurDestId"))).Count();
							ViewBag.DestinationAttractionsPageID = GetCookie("DestinationAttractionsPageNumber");
							ViewBag.DestinationAttractionsPageCount = Count / 20;
							jsonResult = Json(_attractionRep.GetAttractionsOfDestinationForPages(int.Parse(GetCookie("CurDestId")), skip));
						}
					}
					break;
			}

			return jsonResult;
		}
		#endregion

		#region Contents

		[Route("Admin")]
		public async Task<IActionResult> ShowContents(int pageid = 1)
		{
			SetDefaultPageNumbers(3, pageid);
			int skip = (pageid - 1) * 20;
			int Count = _contentRep.GetAllContents().Count();
			ViewBag.ContentsPageID = pageid;
			ViewBag.ContentsPageCount = Count / 20;
			return View(_contentRep.GetContentsForPages(skip));
		}

		public IActionResult AddContent()
		{
			ViewBag.ContentsPageID = GetCookie("ContentsPageNumber");
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddContent(AddEditContent content)
		{

			ViewBag.ContentsPageID = GetCookie("ContentsPageNumber");
			if (ModelState.IsValid)
			{
				Content theContent = new Content()
				{
					ContentTitle = content.ContentTitle,
					ContentType = content.ContentType,
					ContentText = content.ContentText,
					HasImage = (content.ContentImage?.Length > 0) ? true : false,
					ImageExt = (content.ContentImage?.Length > 0) ? Path.GetExtension(content.ContentImage.FileName) : _contentRep.GetContentById(1019).ContentText,
					GeoCoordinates = content.GeoCoordinates
				};
				_contentRep.AddContent(theContent);

				if (content.ContentImage?.Length > 0)
				{
					try
					{
						string filePath = Path.Combine(Directory.GetCurrentDirectory(),
					   "wwwroot",
					   "pics",
					   "sitepics",
						theContent.ContentId.ToString() + Path.GetExtension(content.ContentImage.FileName));
						using (var stream = new FileStream(filePath, FileMode.Create))
						{
							await content.ContentImage.CopyToAsync(stream);
						}
					}
					catch (Exception ex)
					{

						ToolBox.SaveLog(ex.Message);
					}

				}
				return Redirect("/Admin?pageid=" + GetCookie("ContentsPageNumber"));
			}
			return View(content);
		}

		public async Task<IActionResult> EditContent(int id)
		{
			if (id == 0 || _contentRep == null)
			{
				return NotFound();
			}

			AddEditContent contentVM = _contentRep.GetAllContents().Where(c => c.ContentId == id).Select(s => new AddEditContent()
			{
				ContentId = s.ContentId,
				ContentTitle = s.ContentTitle,
				ContentType = s.ContentType,
				ContentText = s.ContentText,
				HasImage = s.HasImage,
				GeoCoordinates = s.GeoCoordinates
			}).FirstOrDefault();
			if (contentVM == null)
			{
				return NotFound();
			}
			ViewBag.ContentsPageID = GetCookie("ContentsPageNumber");
			return View(contentVM);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditContent(int id, AddEditContent content)
		{
			ViewBag.ContentsPageID = GetCookie("ContentsPageNumber");
			if (id != content.ContentId)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				Content theContent = new Content()
				{
					ContentId = content.ContentId,
					ContentTitle = content.ContentTitle,
					ContentType = content.ContentType,
					ContentText = content.ContentText,
					HasImage = (content.ContentImage?.Length > 0) ? true : _contentRep.GetContentById(content.ContentId).HasImage,
					GeoCoordinates = content.GeoCoordinates
					,
					ImageExt = (content.ContentImage?.Length > 0) ? await ChangeImageExt(1, content.ContentId, Path.GetExtension(content.ContentImage.FileName)) : _contentRep.GetContentById(content.ContentId).ImageExt
				};
				_contentRep.EditContent(theContent);
				if (content.ContentImage?.Length > 0)
				{
					try
					{
						await RemoveContentPics(theContent.ContentId);
						string filePath = Path.Combine(Directory.GetCurrentDirectory(),
							"wwwroot",
							"pics",
							"sitepics",
							 theContent.ContentId.ToString() + Path.GetExtension(content.ContentImage.FileName));

						using (var stream = new FileStream(filePath, FileMode.Create))
						{
							await content.ContentImage.CopyToAsync(stream);
						}
					}
					catch (Exception ex)
					{

						ToolBox.SaveLog(ex.Message);
					}
				}
				return Redirect("/Admin?pageid=" + GetCookie("ContentsPageNumber"));
			}
			return View(content);
		}

		private async Task<string?> ChangeImageExt(int v1, int id, string v2)
		{
			switch (v1)
			{
				default:
				case 1:
					await RemoveContentPics(id);
					break;
				case 2:
					await RemoveDestinationPics(id);
					break;
				case 3:
					await RemoveTourPics(id);
					break;
			}
			return v2;
		}

		public async Task<IActionResult> DeleteContent(int id)
		{
			if (id == 0 || _contentRep == null)
			{
				return NotFound();
			}

			var content = _contentRep.GetContentById(id);
			if (content == null)
			{
				return NotFound();
			}
			ViewBag.ContentsPageID = GetCookie("ContentsPageNumber");
			return View(content);
		}

		[HttpPost, ActionName("DeleteContent")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ContentDeleteConfirmed(int id)
		{
			if (_contentRep == null)
			{
				return Problem("Entity set 'FasleSafarContext.Contents'  is null.");
			}
			await RemoveContentPics(id);
			_contentRep.RemoveContent(id);
			return Redirect("/Admin?pageid=" + GetCookie("ContentsPageNumber"));
		}

		private async Task RemoveContentPics(object id)
		{
			var files = GetContentPicsUrls(id);
			if (files.Count > 0)
			{
				foreach (string file in files)
				{
					System.IO.File.Delete(file);
				}
			}
		}

		private List<string> GetContentPicsUrls(object id)
		{
			List<string> ResultUrls = new List<string>();
			string filePath = Path.Combine(Directory.GetCurrentDirectory(),
					  "wwwroot",
					  "pics",
					  "sitepics");
			var files = Directory.GetFiles(filePath, "*.*", SearchOption.AllDirectories);
			if (files.Length > 0)
			{
				foreach (string file in files)
				{
					if (Path.GetFileNameWithoutExtension(file) == id.ToString())
						ResultUrls.Add(file);
				}
			}
			return ResultUrls;
		}

		public IActionResult SetSitePics()
		{
			var infos = new SitePicsVM()
			{
				PhoneNumber = _contentRep.GetContentById(16).ContentText,
				Address = _contentRep.GetContentById(17).ContentText,
				Deposit = int.Parse(_contentRep.GetContentById(1018).ContentText),
				ChildPrice = int.Parse(_contentRep.GetContentById(1023).ContentText),
				BabyPrice = int.Parse(_contentRep.GetContentById(1027).ContentText),
				IranRules = _contentRep.GetContentById(1028).ContentText,
				WorldRules = _contentRep.GetContentById(1029).ContentText,
				Slogan = _contentRep.GetContentById(1030).ContentText,
			};
			return View(infos);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> SetSitePics(SitePicsVM pics)
		{
			if (ModelState.IsValid)
			{
				Content phone = new Content()
				{
					ContentId = 16,
					ContentTitle = "شماره تماس",
					ContentType = "اطلاعات",
					ContentText = pics.PhoneNumber,
					HasImage = false
				};
				_contentRep.EditContent(phone);

				Content address = new Content()
				{
					ContentId = 17,
					ContentTitle = "آدرس",
					ContentType = "اطلاعات",
					ContentText = pics.Address,
					HasImage = false
				};
				_contentRep.EditContent(address);
				Content iranrules = new Content()
				{
					ContentId = 1028,
					ContentTitle = "قوانین داخلی",
					ContentType = "اطلاعات",
					ContentText = pics.IranRules,
					HasImage = false
				};
				_contentRep.EditContent(iranrules);
				Content worldrules = new Content()
				{
					ContentId = 1029,
					ContentTitle = "قوانین خارجی",
					ContentType = "اطلاعات",
					ContentText = pics.WorldRules,
					HasImage = false
				};
				_contentRep.EditContent(worldrules);
				Content slogan = new Content()
				{
					ContentId = 1030,
					ContentTitle = "شعار",
					ContentType = "اطلاعات",
					ContentText = pics.Slogan,
					HasImage = false
				};
				_contentRep.EditContent(slogan);
				Content deposit = new Content()
				{
					ContentId = 1018,
					ContentTitle = "بیعانه",
					ContentType = "اطلاعات",
					ContentText = string.IsNullOrEmpty(pics.Deposit.ToString()) ? "0" : pics.Deposit.ToString(),
					HasImage = false
				};
				_contentRep.EditContent(deposit);
				Content child = new Content()
				{
					ContentId = 1023,
					ContentTitle = "درصد قیمت برای کودکان زیر 12 سال",
					ContentType = "اطلاعات",
					ContentText = string.IsNullOrEmpty(pics.ChildPrice.ToString()) ? "0" : pics.ChildPrice.ToString(),
					HasImage = false
				};
				_contentRep.EditContent(child);
				Content baby = new Content()
				{
					ContentId = 1027,
					ContentTitle = "درصد قیمت برای کودکان زیر 2 سال",
					ContentType = "اطلاعات",
					ContentText = string.IsNullOrEmpty(pics.BabyPrice.ToString()) ? "0" : pics.BabyPrice.ToString(),
					HasImage = false
				};
				_contentRep.EditContent(baby);
			}
			if (pics.LogoImage?.Length > 0)
			{
				string filePath = Path.Combine(Directory.GetCurrentDirectory(),
					"wwwroot",
					"pics",
					"sitepics",
					 "LogoImage" + Path.GetExtension(pics.LogoImage.FileName));
				await RemoveContentPics("LogoImage");
				using (var stream = new FileStream(filePath, FileMode.Create))
				{

					await pics.LogoImage.CopyToAsync(stream);
				}
			}

			if (pics.IconImage?.Length > 0)
			{
				string filePath = Path.Combine(Directory.GetCurrentDirectory(),
					"wwwroot",
					 "favicon" + Path.GetExtension(pics.IconImage.FileName));
				await RemoveContentPics("favicon");
				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await pics.IconImage.CopyToAsync(stream);
				}
			}
			if (pics.DefaultContentImage?.Length > 0)
			{
				string ext = Path.GetExtension(pics.DefaultContentImage.FileName);
				string filePath = Path.Combine(Directory.GetCurrentDirectory(),
					"wwwroot",
					"pics",
					"sitepics",
					 "DefaultContentPic" + Path.GetExtension(pics.DefaultContentImage.FileName));
				await RemoveContentPics("DefaultContentPic");
				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await pics.DefaultContentImage.CopyToAsync(stream);
				}
				Content DefaultContentPicExt = new Content()
				{
					ContentId = 1019,
					ContentTitle = "DefaultContentPicExt",
					ContentType = "اطلاعات",
					ContentText = Path.GetExtension(pics.DefaultContentImage.FileName),
					HasImage = false
				};
				_contentRep.EditContent(DefaultContentPicExt);
			}

			if (pics.BannerImage?.Length > 0)
			{
				string filePath = Path.Combine(Directory.GetCurrentDirectory(),
					"wwwroot",
					"pics",
					"sitepics",
					 "BannerImage" + Path.GetExtension(pics.BannerImage.FileName));
				await RemoveContentPics("BannerImage");
				using (var stream = new FileStream(filePath, FileMode.Create))
				{

					await pics.BannerImage.CopyToAsync(stream);
				}
				Content BannerImageExt = new Content()
				{
					ContentId = 1020,
					ContentTitle = "BannerImageExt",
					ContentType = "اطلاعات",
					ContentText = Path.GetExtension(pics.BannerImage.FileName),
					HasImage = false
				};
				_contentRep.EditContent(BannerImageExt);
			}

			//if (pics.ToursList?.Length > 0)
			//{
			//    string filePath = Path.Combine(Directory.GetCurrentDirectory(),
			//        "wwwroot",
			//        "pics",
			//        "sitepics",
			//         "ToursListImage" + Path.GetExtension(pics.ToursList.FileName));
			//    using (var stream = new FileStream(filePath, FileMode.Create))
			//    {
			//        pics.ToursList.CopyToAsync(stream);
			//    }
			//}

			return View();
		}

		#endregion

		#region Destinations

		public async Task<IActionResult> ShowDestinations(int pageid = 1)
		{
			SetDefaultPageNumbers(1, pageid);
			int skip = (pageid - 1) * 20;
			int Count = _destinationRep.GetAllDestinations().Count();
			ViewBag.DestinationsPageID = pageid;
			ViewBag.DestinationsPageCount = Count / 20;
			return View(_destinationRep.GetDestinationsForPages(skip));
		}

		public IActionResult AddDestination()
		{
			ViewBag.DestinationsPageID = GetCookie("DestinationsPageNumber");
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddDestination(AddDestinationVM destination)
		{
			ViewBag.DestinationsPageID = GetCookie("DestinationsPageNumber");
			if (ModelState.IsValid)
			{
				Destination theDestination = new Destination()
				{
					DestinationName = destination.DestinationName,
					DestinationDescription = destination.DestinationDescription ?? "",
					BigImage = "",
					City = destination.City ?? "",
					Country = destination.Country ?? "",
					Province = destination.Province ?? "",
					GeoCoordinates = destination.GeoCoordinates ?? "",
					ImagesAlbum = "",
					IsAttraction = destination.IsAttraction,
					OnVitrin = destination.OnVitrin
				};
				_destinationRep.AddDestination(theDestination);

				if (destination.BigImage?.Length > 0 || destination.AlbumImages?.Count > 0)
				{
					try
					{
						if (destination.BigImage?.Length > 0)
						{
							string filePath = Path.Combine(Directory.GetCurrentDirectory(),
								"wwwroot",
								"pics",
								"destpics",
								 "d" + theDestination.DestinationId.ToString() + "-" + Path.GetExtension(destination.BigImage.FileName));

							using (var stream = new FileStream(filePath, FileMode.Create))
							{

								await destination.BigImage.CopyToAsync(stream);
							}
							theDestination.BigImage = $"d{theDestination.DestinationId}-{Path.GetExtension(destination.BigImage.FileName)}";
						}
						if (destination.AlbumImages?.Count > 0)
						{
							for (int i = 0; i < destination.AlbumImages.Count; i++)
							{
								string filePath = Path.Combine(Directory.GetCurrentDirectory(),
							   "wwwroot",
							   "pics",
							   "destpics",
								"d" + theDestination.DestinationId.ToString() + "-" + (++i).ToString() + Path.GetExtension(destination.AlbumImages[i].FileName));
								using (var stream = new FileStream(filePath, FileMode.Create))
								{

									await destination.AlbumImages[i].CopyToAsync(stream);
								}
							}
							List<string> fileNames = new List<string>();
							for (int i = 0; i < destination.AlbumImages.Count; i++)
							{
								fileNames.Add($"d{theDestination.DestinationId}-{i + 1}{Path.GetExtension(destination.AlbumImages[i].FileName)}");
							}
							theDestination.ImagesAlbum = string.Join(',', fileNames);
						}
					}
					catch (Exception ex)
					{

						ToolBox.SaveLog(ex.Message);
					}

					_destinationRep.EditDestination(theDestination);
				}
				return Redirect("/Admin/ShowDestinations?pageid=" + GetCookie("DestinationsPageNumber"));
			}
			return View(destination);
		}

		public async Task<IActionResult> EditDestination(int id)
		{
			if (id == 0 || _destinationRep == null)
			{
				return NotFound();
			}

			EditDestinationVM destinationVM = _destinationRep.GetAllDestinations().Where(c => c.DestinationId == id).Select(s => new EditDestinationVM()
			{
				DestinationId = s.DestinationId,
				DestinationName = s.DestinationName,
				DestinationDescription = s.DestinationDescription,
				BigImage = s.BigImage,
				ImagesAlbum = s.ImagesAlbum,
				City = s.City,
				Country = s.Country,
				GeoCoordinates = s.GeoCoordinates,
				IsAttraction = s.IsAttraction,
				OnVitrin = s.OnVitrin,
				Province = s.Province
			}).FirstOrDefault();
			if (destinationVM == null)
			{
				return NotFound();
			}
			ViewBag.DestinationsPageID = GetCookie("DestinationsPageNumber");
			return View(destinationVM);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditDestination(int id, EditDestinationVM destination)
		{
			ViewBag.DestinationsPageID = GetCookie("DestinationsPageNumber");
			if (id != destination.DestinationId)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				Destination theDestination = new Destination()
				{
					DestinationId = destination.DestinationId,
					DestinationName = destination.DestinationName,
					DestinationDescription = destination.DestinationDescription ?? "",
					BigImage = destination.BigImage ?? "",
					ImagesAlbum = destination.ImagesAlbum ?? "",
					City = destination.City ?? "",
					Country = destination.Country ?? "",
					GeoCoordinates = destination.GeoCoordinates ?? "",
					IsAttraction = destination.IsAttraction,
					Province = destination.Province ?? "",
					OnVitrin = destination.OnVitrin
				};
				if (destination.BigDestinationImage?.Length > 0 || destination.AlbumImages?.Count > 0)
				{

					try
					{
						if (destination.BigDestinationImage?.Length > 0)
						{
							await RemoveDestinationPics(theDestination.DestinationId, 2);
							string filePath = Path.Combine(Directory.GetCurrentDirectory(),
								"wwwroot",
								"pics",
								"destpics",
								 "d" + theDestination.DestinationId.ToString() + "-" + Path.GetExtension(destination.BigDestinationImage.FileName));
							using (var stream = new FileStream(filePath, FileMode.Create))
							{

								await destination.BigDestinationImage.CopyToAsync(stream);
							}
							theDestination.BigImage = $"d{destination.DestinationId}-{Path.GetExtension(destination.BigDestinationImage.FileName)}";
						}
						if (destination.AlbumImages?.Count > 0)
						{
							await RemoveDestinationPics(theDestination.DestinationId, 3);
							for (int i = 0; i < destination.AlbumImages.Count; i++)
							{
								string filePath = Path.Combine(Directory.GetCurrentDirectory(),
							   "wwwroot",
							   "pics",
							   "destpics",
								"d" + theDestination.DestinationId.ToString() + "-" + (++i).ToString() + Path.GetExtension(destination.AlbumImages[i].FileName));
								using (var stream = new FileStream(filePath, FileMode.Create))
								{

									await destination.AlbumImages[i].CopyToAsync(stream);
								}
							}
							List<string> fileNames = new List<string>();
							for (int i = 0; i < destination.AlbumImages.Count; i++)
							{
								fileNames.Add($"d{destination.DestinationId}-{i + 1}{Path.GetExtension(destination.AlbumImages[i].FileName)}");
							}
							theDestination.ImagesAlbum = string.Join(',', fileNames);
						}
					}
					catch (Exception ex)
					{

						ToolBox.SaveLog(ex.Message);
					}
				}
				_destinationRep.EditDestination(theDestination);
				return Redirect("/Admin/ShowDestinations?pageid=" + GetCookie("DestinationsPageNumber"));
			}
			return View(destination);
		}

		public async Task<IActionResult> DeleteDestination(int id)
		{
			if (id == 0 || _destinationRep == null)
			{
				return NotFound();
			}

			var destination = _destinationRep.GetDestinationById(id);
			if (destination == null)
			{
				return NotFound();
			}
			ViewBag.DestinationsPageID = GetCookie("DestinationsPageNumber");
			return View(destination);
		}

		[HttpPost, ActionName("DeleteDestination")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DestinationDeleteConfirmed(int id)
		{
			if (_destinationRep == null)
			{
				return Problem("Entity set 'FasleSafarContext.Destinations'  is null.");
			}
			await RemoveDestinationPics(id);
			_destinationRep.RemoveDestination(id);
			return Redirect("/Admin/ShowDestinations?pageid=" + GetCookie("DestinationsPageNumber"));
		}

		private async Task RemoveDestinationPics(int id, int mode = 1)
		{
			var files = GetDestinationPicsUrls(id);
			if (files.Count > 0)
			{
				switch (mode)
				{
					case 1:
					default:
						foreach (string file in files)
						{
							System.IO.File.Delete(file);
						}
						break;
					case 2:
						if (Path.GetFileNameWithoutExtension(files[0]).EndsWith('-'))
							System.IO.File.Delete(files[0]);
						break;
					case 3:
						foreach (string file in files)
						{
							if (!Path.GetFileNameWithoutExtension(file).EndsWith('-'))
								System.IO.File.Delete(file);
						}
						break;
				}
			}
		}

		private List<string> GetDestinationPicsUrls(int id)
		{
			List<string> ResultUrls = new List<string>();
			string filePath = Path.Combine(Directory.GetCurrentDirectory(),
					  "wwwroot",
					  "pics",
					  "destpics");
			var files = Directory.GetFiles(filePath, "*.*", SearchOption.AllDirectories);
			if (files.Length > 0)
			{
				foreach (string file in files)
				{
					if (Path.GetFileNameWithoutExtension(file).Contains($"d{id}-"))
						ResultUrls.Add(file);
				}
			}
			return ResultUrls;
		}

		#endregion

		#region Attractions

		public async Task<IActionResult> ShowAttractions(int pageid = 1)
		{
			SetDefaultPageNumbers(14, pageid);
			int skip = (pageid - 1) * 20;
			int Count = _attractionRep.GetAllAttractions().Count();
			ViewBag.AttractionsPageID = pageid;
			ViewBag.AttractionsPageCount = Count / 20;
			return View(_attractionRep.GetAttractionsForPages(skip));
		}

		public async Task<IActionResult> ShowDestinationAttractions(int id, int pageid = 1)
		{
			SetCookie("CurDestId", id.ToString());
			ViewBag.DestinationId = id;
			ViewBag.DestinationName = _destinationRep.GetDestinationById(id).DestinationName;
			SetDefaultPageNumbers(15, pageid);
			int skip = (pageid - 1) * 20;
			int Count = _tourRep.GetToursOfDestination(id).Count();
			ViewBag.DestinationToursPageID = pageid;
			ViewBag.DestinationToursPageCount = Count / 20;
			return View(_attractionRep.GetAttractionsOfDestination(id));
		}

		public IActionResult AddAttraction()
		{
			ViewBag.AttractionsPageID = GetCookie("AttractionsPageNumber");
			ViewBag.Destinations = _destinationRep.GetAllDestinations();
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddAttraction(AddAttractionVM attraction)
		{
			ViewBag.AttractionsPageID = GetCookie("AttractionsPageNumber");
			ViewBag.Destinations = _destinationRep.GetAllDestinations();
			if (ModelState.IsValid)
			{
				Attraction theAttraction = new Attraction()
				{
					AttractionName = attraction.AttractionName,
					AttractionDescription = attraction.AttractionDescription ?? "",
					BigImage = "",
					GeoCoordinates = attraction.GeoCoordinates ?? "",
					ImagesAlbum = "",
					DestinationId = attraction.DestinationId,
				};
				_attractionRep.AddAttraction(theAttraction);

				if (attraction.BigImage?.Length > 0 || attraction.AlbumImages?.Count > 0)
				{
					try
					{
						if (attraction.BigImage?.Length > 0)
						{
							string filePath = Path.Combine(Directory.GetCurrentDirectory(),
								"wwwroot",
								"pics",
								"attrpics",
								 "a" + theAttraction.AttractionId.ToString() + "-" + Path.GetExtension(attraction.BigImage.FileName));
							using (var stream = new FileStream(filePath, FileMode.Create))
							{
								await attraction.BigImage.CopyToAsync(stream);
							}
							theAttraction.BigImage = $"a{theAttraction.AttractionId}-{Path.GetExtension(attraction.BigImage.FileName)}";
						}
						if (attraction.AlbumImages?.Count > 0)
						{
							for (int i = 0; i < attraction.AlbumImages.Count; i++)
							{

								string filePath = Path.Combine(Directory.GetCurrentDirectory(),
						"wwwroot",
						"pics",
						"attrpics",
						 "a" + theAttraction.AttractionId.ToString() + "-" + (++i).ToString() + Path.GetExtension(attraction.AlbumImages[i].FileName));
								using (var stream = new FileStream(filePath, FileMode.Create))
								{

									await attraction.AlbumImages[i].CopyToAsync(stream);
								}


							}
							List<string> fileNames = new List<string>();
							for (int i = 0; i < attraction.AlbumImages.Count; i++)
							{
								fileNames.Add($"a{theAttraction.AttractionId}-{i + 1}{Path.GetExtension(attraction.AlbumImages[i].FileName)}");
							}
							theAttraction.ImagesAlbum = string.Join(',', fileNames);
						}
					}
					catch (Exception ex)
					{

						ToolBox.SaveLog(ex.Message);
					}

					_attractionRep.EditAttraction(theAttraction);
				}
				return Redirect("/Admin/ShowAttractions?pageid=" + GetCookie("AttractionsPageNumber"));
			}
			return View(attraction);
		}

		public async Task<IActionResult> EditAttraction(int id)
		{
			if (id == 0 || _attractionRep == null)
			{
				return NotFound();
			}

			EditAttractionVM attractionVM = _attractionRep.GetAllAttractions().Where(c => c.AttractionId == id).Select(s => new EditAttractionVM()
			{
				AttractionId = s.AttractionId,
				AttractionName = s.AttractionName,
				AttractionDescription = s.AttractionDescription,
				BigImage = s.BigImage,
				ImagesAlbum = s.ImagesAlbum,
				GeoCoordinates = s.GeoCoordinates,
				DestinationId = (int)s.DestinationId,
			}).FirstOrDefault();
			if (attractionVM == null)
			{
				return NotFound();
			}
			ViewBag.AttractionsPageID = GetCookie("AttractionsPageNumber");
			ViewBag.Destinations = _destinationRep.GetAllDestinations();
			return View(attractionVM);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditAttraction(int id, EditAttractionVM attraction)
		{
			ViewBag.AttractionsPageID = GetCookie("AttractionsPageNumber");
			ViewBag.Destinations = _destinationRep.GetAllDestinations();
			if (id != attraction.AttractionId)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				Attraction theAttraction = new Attraction()
				{
					AttractionId = attraction.AttractionId,
					AttractionName = attraction.AttractionName,
					AttractionDescription = attraction.AttractionDescription ?? "",
					BigImage = attraction.BigImage ?? "",
					ImagesAlbum = attraction.ImagesAlbum ?? "",
					GeoCoordinates = attraction.GeoCoordinates ?? "",
					DestinationId = (int)attraction.DestinationId
				};
				if (attraction.BigAttractionImage?.Length > 0 || attraction.AlbumImages?.Count > 0)
				{
					try
					{
						if (attraction.BigAttractionImage?.Length > 0)
						{
							await RemoveAttractionPics(theAttraction.AttractionId, 2);
							string filePath = Path.Combine(Directory.GetCurrentDirectory(),
								"wwwroot",
								"pics",
								"attrpics",
								 "a" + theAttraction.AttractionId.ToString() + "-" + Path.GetExtension(attraction.BigAttractionImage.FileName));
							using (var stream = new FileStream(filePath, FileMode.Create))
							{

								await attraction.BigAttractionImage.CopyToAsync(stream);
							}
							theAttraction.BigImage = $"a{attraction.AttractionId}-{Path.GetExtension(attraction.BigAttractionImage.FileName)}";
						}
						if (attraction.AlbumImages?.Count > 0)
						{
							await RemoveAttractionPics(theAttraction.AttractionId, 3);
							for (int i = 0; i < attraction.AlbumImages.Count; i++)
							{
								string filePath = Path.Combine(Directory.GetCurrentDirectory(),
							   "wwwroot",
							   "pics",
							   "attrpics",
								"a" + theAttraction.AttractionId.ToString() + "-" + (++i).ToString() + Path.GetExtension(attraction.AlbumImages[i].FileName));
								using (var stream = new FileStream(filePath, FileMode.Create))
								{

									await attraction.AlbumImages[i].CopyToAsync(stream);
								}
							}
							List<string> fileNames = new List<string>();
							for (int i = 0; i < attraction.AlbumImages.Count; i++)
							{
								fileNames.Add($"a{attraction.AttractionId}-{i + 1}{Path.GetExtension(attraction.AlbumImages[i].FileName)}");
							}
							theAttraction.ImagesAlbum = string.Join(',', fileNames);
						}
					}
					catch (Exception ex)
					{

						ToolBox.SaveLog(ex.Message);
					}
				}
				_attractionRep.EditAttraction(theAttraction);
				return Redirect("/Admin/ShowAttractions?pageid=" + GetCookie("AttractionsPageNumber"));
			}
			return View(attraction);
		}

		public async Task<IActionResult> DeleteAttraction(int id)
		{
			if (id == 0 || _attractionRep == null)
			{
				return NotFound();
			}

			var attraction = _attractionRep.GetAttractionById(id);
			if (attraction == null)
			{
				return NotFound();
			}
			ViewBag.AttractionsPageID = GetCookie("AttractionsPageNumber");
			return View(attraction);
		}

		[HttpPost, ActionName("DeleteAttraction")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AttractionDeleteConfirmed(int id)
		{
			if (_attractionRep == null)
			{
				return Problem("Entity set 'FasleSafarContext.Attractions'  is null.");
			}
			await RemoveAttractionPics(id);
			_attractionRep.RemoveAttraction(id);
			return Redirect("/Admin/ShowAttractions?pageid=" + GetCookie("AttractionsPageNumber"));
		}

		private async Task RemoveAttractionPics(int id, int mode = 1)
		{
			var files = GetAttractionPicsUrls(id);
			if (files.Count > 0)
			{
				switch (mode)
				{
					case 1:
					default:
						foreach (string file in files)
						{
							System.IO.File.Delete(file);
						}
						break;
					case 2:
						if (Path.GetFileNameWithoutExtension(files[0]).EndsWith('-'))
							System.IO.File.Delete(files[0]);
						break;
					case 3:
						foreach (string file in files)
						{
							if (!Path.GetFileNameWithoutExtension(file).EndsWith('-'))
								System.IO.File.Delete(file);
						}
						break;
				}
			}
		}

		private List<string> GetAttractionPicsUrls(int id)
		{
			List<string> ResultUrls = new List<string>();
			string filePath = Path.Combine(Directory.GetCurrentDirectory(),
					  "wwwroot",
					  "pics",
					  "attrpics");
			var files = Directory.GetFiles(filePath, "*.*", SearchOption.AllDirectories);
			if (files.Length > 0)
			{
				foreach (string file in files)
				{
					if (Path.GetFileNameWithoutExtension(file).Contains($"a{id}-"))
						ResultUrls.Add(file);
				}
			}
			return ResultUrls;
		}

		#endregion

		#region Tours

		public async Task<IActionResult> ShowTours(int pageid = 1)
		{
			SetDefaultPageNumbers(9, pageid);
			int skip = (pageid - 1) * 20;
			int Count = _tourRep.GetAllTours().Count();
			ViewBag.ToursPageID = pageid;
			ViewBag.ToursPageCount = Count / 20;
			List<TourVM> tourVMs = _tourRep.GetToursForPages(skip).Select(t => new TourVM()
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
				Vehicle = t.Vehicle,
				Destination = t.Destination,
				IsLeasing = t.IsLeasing,
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
		public async Task<IActionResult> ShowDestinationTours(int id, int pageid = 1)
		{
			SetCookie("CurDestId", id.ToString());
			ViewBag.DestinationId = id;
			ViewBag.DestinationTitle = _destinationRep.GetDestinationById(id).DestinationName;
			SetDefaultPageNumbers(2, pageid);
			int skip = (pageid - 1) * 20;
			int Count = _tourRep.GetToursOfDestination(id).Count();
			ViewBag.DestinationToursPageID = pageid;
			ViewBag.DestinationToursPageCount = Count / 20;
			List<TourVM> tourVMs = _tourRep.GetToursOfDestinationForPages(id, skip).Select(t => new TourVM()
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
				Vehicle = t.Vehicle,
				Destination = t.Destination,
				IsLeasing = t.IsLeasing,
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
		public async Task<IActionResult> ShowUserTours(int id, int pageid = 1)
		{
			SetCookie("CurUserId", id.ToString());
			ViewBag.UserId = id;
			ViewBag.FullName = _userRep.GetUserById(id).FullName;
			SetDefaultPageNumbers(8, pageid);
			int skip = (pageid - 1) * 20;
			int Count = _tourRep.GetToursOfUser(id).Count();
			ViewBag.UserToursPageID = pageid;
			ViewBag.UserToursPageCount = Count / 20;
			List<TourVM> tourVMs = _tourRep.GetToursOfUserForPages(id, skip).Select(t => new TourVM()
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
				IsLeasing = t.IsLeasing,
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

		public IActionResult AddTour()
		{
			ViewBag.ToursPageID = GetCookie("ToursPageNumber");
			ViewBag.Destinations = _destinationRep.GetAllDestinations();
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddTour(AddTourVM tour)
		{
			ViewBag.ToursPageID = GetCookie("ToursPageNumber");
			ViewBag.Destinations = _destinationRep.GetAllDestinations();
			if (ModelState.IsValid)
			{
				Tour theTour = new Tour()
				{
					TourTitle = tour.TourTitle,
					Capacity = tour.Capacity,
					DaysCount = tour.DaysCount,
					DestinationId = tour.DestinationId,
					EndDate = tour.EndDate,
					OpenState = "فعال",
					Price = GetPrice(tour.ThreeStarPrice, tour.FourStarPrice, tour.FiveStarPrice),
					AvgScore = 0,
					ScoreCount = 0,
					TotalScore = 0,
					StartDate = tour.StartDate,
					TourDescription = tour.TourDescription,
					TourType = tour.TourType,
					TransportType = tour.TransportType,
					Vehicle = tour.Vehicle ?? "",
					IsLeasing = tour.IsLeasing,
					BigImage = "",
					ImagesAlbum = "",
					//SmallImage = "",
					ExcludeCosts = tour.ExcludeCosts ?? "",
					ReturnTime = tour.ReturnTime ?? "",
					Attractions = tour.Attractions ?? "",
					Facilities = tour.Facilities ?? "",
					IncludeCosts = tour.IncludeCosts ?? "",
					ReachTime = tour.ReachTime ?? "",
					GeoCoordinates = tour.GeoCoordinates.Trim()
				};
				_tourRep.AddTour(theTour);
				_tourRep.PutPricesOfTour(theTour.TourId, tour.Price1Title, tour.Price2Title, tour.Price3Title, tour.ThreeStarPrice, tour.FourStarPrice, tour.FiveStarPrice);
				if (tour.BigTourImage?.Length > 0 || tour.AlbumImages?.Count > 0)
				{
					try
					{
						if (tour.BigTourImage?.Length > 0)
						{
							string filePath = Path.Combine(Directory.GetCurrentDirectory(),
								"wwwroot",
								"pics",
								"tourpics",
								 "t" + theTour.TourId.ToString() + "-" + Path.GetExtension(tour.BigTourImage.FileName));
							using (var stream = new FileStream(filePath, FileMode.Create))
							{

								await tour.BigTourImage.CopyToAsync(stream);
							}
							theTour.BigImage = $"t{theTour.TourId}-{Path.GetExtension(tour.BigTourImage.FileName)}";
						}
						if (tour.AlbumImages?.Count > 0)
						{
							for (int i = 0; i < tour.AlbumImages.Count; i++)
							{
								string filePath = Path.Combine(Directory.GetCurrentDirectory(),
							   "wwwroot",
							   "pics",
							   "tourpics",
								"t" + theTour.TourId.ToString() + "-" + (++i).ToString() + Path.GetExtension(tour.AlbumImages[i].FileName));
								using (var stream = new FileStream(filePath, FileMode.Create))
								{

									await tour.AlbumImages[i].CopyToAsync(stream);
								}
							}
							List<string> fileNames = new List<string>();
							for (int i = 0; i < tour.AlbumImages.Count; i++)
							{
								fileNames.Add($"t{theTour.TourId}-{i + 1}{Path.GetExtension(tour.AlbumImages[i].FileName)}");
							}
							theTour.ImagesAlbum = string.Join(',', fileNames);
						}
					}
					catch (Exception ex)
					{

						ToolBox.SaveLog(ex.Message);
					}
					_tourRep.EditTour(theTour);
				}

				//if (tour.SmallTourImage?.Length > 0)
				//{
				//    string filePath = Path.Combine(Directory.GetCurrentDirectory(),
				//        "wwwroot",
				//        "pics",
				//        "tourpics",
				//         "t" + theTour.TourId.ToString() +  "-small" + Path.GetExtension(tour.SmallTourImage.FileName));
				//    using (var stream = new FileStream(filePath, FileMode.Create))
				//    {
				//        tour.SmallTourImage.CopyToAsync(stream);
				//    }
				//}
				return Redirect("/Admin/ShowTours?pageid=" + GetCookie("ToursPageNumber"));
			}
			return View(tour);
		}

		private decimal GetPrice(string price3, string price4, string price5)
		{
			string finalPrice = "";
			if (!string.IsNullOrEmpty(price3) && int.Parse(price3[0].ToString()) > 0)
				finalPrice = price3;
			else if (!string.IsNullOrEmpty(price4) && int.Parse(price4[0].ToString()) > 0)
				finalPrice = price4;
			else if (!string.IsNullOrEmpty(price5) && int.Parse(price5[0].ToString()) > 0)
				finalPrice = price5;
			if (finalPrice == "") finalPrice = "0";

			return RetreivePrice(finalPrice);
		}

		private decimal RetreivePrice(string finalPrice)
		{
			for (int i = 0; i < finalPrice.Length; i++)
			{
				if (finalPrice[i] == ',') finalPrice.Remove(i, 1);
			}
			return decimal.Parse(finalPrice);
		}

		public async Task<IActionResult> EditTour(int id)
		{

			if (id == 0 || _tourRep == null)
			{
				return NotFound();
			}
			var arr = _tourRep.GetHotelStaringsOfTour(id);
			EditTourVM TourVM = _tourRep.GetAllTours().Where(c => c.TourId == id).Select(s => new EditTourVM()
			{
				TourId = s.TourId,
				TourTitle = s.TourTitle,
				Capacity = s.Capacity,
				DaysCount = s.DaysCount,
				DestinationId = (int)s.DestinationId,
				EndDate = s.EndDate,
				OpenState = s.OpenState,
				ThreeStarPrice = arr[0].Price,
				FourStarPrice = arr[1].Price,
				FiveStarPrice = arr[2].Price,
				Price1Title = arr[0].Title,
				Price2Title = arr[1].Title,
				Price3Title = arr[2].Title,
				StartDate = s.StartDate,
				TourDescription = s.TourDescription,
				TourType = s.TourType,
				TransportType = s.TransportType,
				Vehicle = s.Vehicle ?? "",
				IsLeasing = s.IsLeasing,
				IncludeCosts = s.IncludeCosts,
				Attractions = s.Attractions,
				ExcludeCosts = s.ExcludeCosts,
				Facilities = s.Facilities,
				ReachTime = s.ReachTime,
				ReturnTime = s.ReturnTime,
				ImagesAlbum = s.ImagesAlbum,
				BigImage = s.BigImage,
				SmallImage = s.SmallImage,
				GeoCoordinates = s.GeoCoordinates ?? ""
			}).FirstOrDefault();
			if (TourVM == null)
			{
				return NotFound();
			}
			ViewBag.ToursPageID = GetCookie("ToursPageNumber");
			ViewBag.Destinations = _destinationRep.GetAllDestinations();
			return View(TourVM);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditTour(int id, EditTourVM tour)
		{
			ViewBag.ToursPageID = GetCookie("ToursPageNumber");
			ViewBag.Destinations = _destinationRep.GetAllDestinations();
			if (id != tour.TourId)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				Tour theTour = new Tour()
				{
					TourId = tour.TourId,
					TourTitle = tour.TourTitle,
					Capacity = tour.Capacity,
					DaysCount = tour.DaysCount,
					DestinationId = tour.DestinationId,
					EndDate = tour.EndDate,
					OpenState = (tour.Capacity < 1) ? "پایان یافته" : tour.OpenState,
					Price = GetPrice(tour.ThreeStarPrice, tour.FourStarPrice, tour.FiveStarPrice),
					AvgScore = _tourRep.GetTourById(tour.TourId).AvgScore,
					TotalScore = _tourRep.GetTourById(tour.TourId).TotalScore,
					ScoreCount = _tourRep.GetTourById(tour.TourId).ScoreCount,
					StartDate = tour.StartDate,
					TourDescription = tour.TourDescription,
					TourType = tour.TourType,
					TransportType = tour.TransportType,
					Vehicle = tour.Vehicle ?? "",
					IsLeasing = tour.IsLeasing,
					//SmallImage = tour.SmallTourImage?.FileName,
					ExcludeCosts = tour.ExcludeCosts ?? "",
					ReturnTime = tour.ReturnTime ?? "",
					Attractions = tour.Attractions ?? "",
					Facilities = tour.Facilities ?? "",
					IncludeCosts = tour.IncludeCosts ?? "",
					ReachTime = tour.ReachTime ?? "",
					GeoCoordinates = tour.GeoCoordinates.Trim(),
					BigImage = tour.BigImage,
					ImagesAlbum = tour.ImagesAlbum ?? ""
				};
				if (tour.BigTourImage?.Length > 0 || tour.AlbumImages?.Count > 0)
				{
					try
					{
						if (tour.BigTourImage?.Length > 0)
						{
							await RemoveTourPics(theTour.TourId, 2);
							string filePath = Path.Combine(Directory.GetCurrentDirectory(),
								"wwwroot",
								"pics",
								"tourpics",
								 "t" + theTour.TourId.ToString() + "-" + Path.GetExtension(tour.BigTourImage.FileName));
							using (var stream = new FileStream(filePath, FileMode.Create))
							{

								await tour.BigTourImage.CopyToAsync(stream);
							}
							theTour.BigImage = $"t{tour.TourId}-{Path.GetExtension(tour.BigTourImage.FileName)}";
						}
						if (tour.AlbumImages?.Count > 0)
						{
							await RemoveTourPics(theTour.TourId, 3);
							for (int i = 0; i < tour.AlbumImages.Count; i++)
							{
								string filePath = Path.Combine(Directory.GetCurrentDirectory(),
							   "wwwroot",
							   "pics",
							   "tourpics",
								"t" + theTour.TourId.ToString() + "-" + (i + 1).ToString() + Path.GetExtension(tour.AlbumImages[i].FileName));
								using (var stream = new FileStream(filePath, FileMode.Create))
								{

									await tour.AlbumImages[i].CopyToAsync(stream);
								}
							}
							List<string> fileNames = new List<string>();
							for (int i = 0; i < tour.AlbumImages.Count; i++)
							{
								fileNames.Add($"t{tour.TourId}-{i + 1}{Path.GetExtension(tour.AlbumImages[i].FileName)}");
							}
							theTour.ImagesAlbum = string.Join(',', fileNames);
						}
					}
					catch (Exception ex)
					{

						ToolBox.SaveLog(ex.Message);
					}
				}
				//if (tour.SmallTourImage?.Length > 0)
				//{
				//    string filePath = Path.Combine(Directory.GetCurrentDirectory(),
				//        "wwwroot",
				//        "pics",
				//        "tourpics",
				//         "t" + theTour.TourId.ToString() + "-small" + Path.GetExtension(tour.SmallTourImage.FileName));
				//    using (var stream = new FileStream(filePath, FileMode.Create))
				//    {
				//        tour.SmallTourImage.CopyToAsync(stream);
				//    }
				//}

				_tourRep.EditTour(theTour);
				_tourRep.PutPricesOfTour(theTour.TourId, tour.Price1Title, tour.Price2Title, tour.Price3Title, tour.ThreeStarPrice, tour.FourStarPrice, tour.FiveStarPrice);
				if (theTour.OpenState == "لغو شده")
				{
					SendCanceledTourMessage(theTour);
				}
				return Redirect("/Admin/ShowTours?pageid=" + GetCookie("ToursPageNumber"));
			}
			return View(tour);
		}

		private async void SendCanceledTourMessage(Tour theTour)
		{
			var users = _userRep.GetUsersOfTour(theTour.TourId);
			foreach (var user in users)
			{
				string message = MakeCanceledTourMessage(user.FullName, theTour.TourTitle);
				bool sent = await SendMessage(user.MobileNumber, message);
			}
		}

		private string MakeCanceledTourMessage(string fullName, string tourTitle)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine($"{fullName} عزیز!");
			sb.AppendLine($"متاسفانه تور {tourTitle} لغو و به زمان دیگری موکول گردید.");
			sb.AppendLine($"جهت اطلاعات بیشتر با آژانس مسافرتی فصل سفر اصفهان تماس بگیرید");
			sb.AppendLine($"در سفر های آینده همراه ما باشید.");
			sb.AppendLine($"{_contentRep.GetContentById(1030).ContentText}");
			sb.AppendLine("آژانس مسافرتی فصل سفر اصفهان");
			return sb.ToString();
		}

		public async Task<IActionResult> DeleteTour(int id)
		{
			if (id == 0 || _tourRep == null)
			{
				return NotFound();
			}

			var tour = _tourRep.GetTourById(id);
			if (tour == null)
			{
				return NotFound();
			}
			var arr = _tourRep.GetHotelStaringsOfTour(id);
			ViewBag.Price3 = arr[0].Price;
			ViewBag.Price4 = arr[1].Price;
			ViewBag.Price5 = arr[2].Price;
			ViewBag.Title3 = arr[0].Title;
			ViewBag.Title4 = arr[1].Title;
			ViewBag.Title5 = arr[2].Title;
			ViewBag.ToursPageID = GetCookie("ToursPageNumber");
			return View(tour);
		}

		[HttpPost, ActionName("DeleteTour")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> TourDeleteConfirmed(int id)
		{
			if (_tourRep == null)
			{
				return Problem("Entity set 'FasleSafarContext.Tours'  is null.");
			}
			await RemoveTourPics(id);
			_tourRep.RemoveTour(id);
			return Redirect("/Admin/ShowTours?pageid=" + GetCookie("ToursPageNumber"));
		}

		private async Task RemoveTourPics(int id, int mode = 1)
		{
			var files = GetTourPicsUrls(id);
			if (files.Count > 0)
			{
				switch (mode)
				{
					case 1:
					default:
						foreach (string file in files)
						{
							System.IO.File.Delete(file);
						}
						break;
					case 2:
						if (Path.GetFileNameWithoutExtension(files[0]).EndsWith('-'))
							System.IO.File.Delete(files[0]);
						break;
					case 3:
						foreach (string file in files)
						{
							if (!Path.GetFileNameWithoutExtension(file).EndsWith('-'))
								System.IO.File.Delete(file);
						}
						break;
				}
			}
		}

		private List<string> GetTourPicsUrls(int id)
		{
			List<string> ResultUrls = new List<string>();
			string filePath = Path.Combine(Directory.GetCurrentDirectory(),
					  "wwwroot",
					  "pics",
					  "tourpics");
			var files = Directory.GetFiles(filePath, "*.*", SearchOption.AllDirectories);
			if (files.Length > 0)
			{
				foreach (string file in files)
				{
					if (Path.GetFileNameWithoutExtension(file).Contains($"t{id}-"))
						ResultUrls.Add(file);
				}
			}
			return ResultUrls;
		}

		#endregion

		#region Users

		public async Task<IActionResult> ShowUsers(int pageid = 1)
		{
			SetDefaultPageNumbers(4, pageid);
			int skip = (pageid - 1) * 20;
			int Count = _userRep.GetAllUsers().Count();
			ViewBag.UsersPageID = pageid;
			ViewBag.UsersPageCount = Count / 20;
			return View(_userRep.GetUsersForPages(skip));
		}
		public async Task<IActionResult> ShowTourUsers(int id, int pageid = 1)
		{
			ViewBag.TourId = id;
			ViewBag.TourTitle = _tourRep.GetTourById(id).TourTitle;
			SetDefaultPageNumbers(7, pageid);
			int skip = (pageid - 1) * 20;
			int Count = _userRep.GetUsersOfTour(id).Count();
			ViewBag.TourUsersPageID = pageid;
			ViewBag.TourUsersPageCount = Count / 20;
			return View(_userRep.GetUsersOfTourForPages(id, skip));
		}

		public IActionResult AddUser()
		{
			ViewBag.UsersPageID = GetCookie("UsersPageNumber");
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddUser(AddEditUserVM user)
		{
			ViewBag.UsersPageID = GetCookie("UsersPageNumber");
			if (ModelState.IsValid)
			{
				User theUser = new User()
				{
					Email = user.Email.ToLower(),
					FullName = user.FullName,
					IsAdmin = user.IsAdmin,
					MobileNumber = user.MobileNumber,
					Password = user.Password
				};
				_userRep.AddUser(theUser);
				return Redirect("/Admin/ShowUsers?pageid=" + GetCookie("ToursPageNumber"));
			}
			return View(user);
		}

		public async Task<IActionResult> EditUser(int id)
		{
			if (id == 0 || _userRep == null)
			{
				return NotFound();
			}

			AddEditUserVM UserVM = _userRep.GetAllUsers().Where(c => c.UserId == id).Select(s => new AddEditUserVM()
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
			ViewBag.UsersPageID = GetCookie("UsersPageNumber");
			return View(UserVM);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditUser(int id, AddEditUserVM user)
		{
			//smsSendWebServiceSoap sms = new smsSendWebServiceSoapClient();

			ViewBag.UsersPageID = GetCookie("UsersPageNumber");
			if (id != user.UserId)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				User theUser = new User()
				{
					UserId = user.UserId,
					Email = user.Email.ToLower(),
					FullName = user.FullName,
					IsAdmin = user.IsAdmin,
					MobileNumber = user.MobileNumber,
					Password = user.Password
				};
				_userRep.EditUser(theUser);
				return Redirect("/Admin/ShowUsers?pageid=" + GetCookie("UsersPageNumber"));
			}
			return View(user);
		}

		public async Task<IActionResult> DeleteUser(int id)
		{
			if (id == 0 || _userRep == null)
			{
				return NotFound();
			}

			var User = _userRep.GetUserById(id);
			if (User == null)
			{
				return NotFound();
			}
			ViewBag.UsersPageID = GetCookie("UsersPageNumber");
			return View(User);
		}

		[HttpPost, ActionName("DeleteUser")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> UserDeleteConfirmed(int id)
		{
			if (_userRep == null)
			{
				return Problem("Entity set 'FasleSafarContext.Users'  is null.");
			}
			_userRep.RemoveUser(id);
			return Redirect("/Admin/ShowUsers?pageid=" + GetCookie("UsersPageNumber"));
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

		#endregion

		#region Orders

		public async Task<IActionResult> ShowOrders(int pageid = 1)
		{
			SetDefaultPageNumbers(5, pageid);
			int skip = (pageid - 1) * 20;
			int Count = _orderRep.GetAllOrders().Count();
			ViewBag.OrdersPageID = pageid;
			ViewBag.OrdersPageCount = Count / 20;
			return View(_orderRep.GetOrdersForPages(skip));
		}
		public async Task<IActionResult> ShowUserOrders(int id, int pageid = 1)
		{
			SetCookie("CurUserId", id.ToString());
			ViewBag.UserId = id;
			ViewBag.FullName = _userRep.GetUserById(id).FullName;
			SetDefaultPageNumbers(6, pageid);
			int skip = (pageid - 1) * 20;
			int Count = _orderRep.GetOrdersByUserId(id).Count;
			ViewBag.UserOrdersPageID = pageid;
			ViewBag.UserOrdersPageCount = Count / 20;
			return View(_orderRep.GetOrdersOfUserForPages(id, skip));
		}

		public async Task<IActionResult> EditState(int id)
		{
			if (id == 0 || _orderRep == null)
			{
				return NotFound();
			}
			Order order = _orderRep.GetOrderById(id);

			EditStateVM stateVM = new EditStateVM()
			{
				OrderId = (int)order.OrderId,
				UserName = order.User.FullName,
				PayState = order.IsFinaly
			};
			if (stateVM == null)
			{
				return NotFound();
			}
			ViewBag.OrdersPageID = GetCookie("OrdersPageNumber");
			return View(stateVM);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditState(int id, EditStateVM stateVM)
		{
			ViewBag.OrdersPageID = GetCookie("OrdersPageNumber");
			if (id != stateVM.OrderId)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				var order = _orderRep.GetOrderById(id);
				order.IsFinaly = stateVM.PayState;
				_orderRep.EditOrder(order);
				return Redirect("/Admin/ShowOrders?pageid=" + GetCookie("OrdersPageNumber"));
			}
			return View(stateVM);
		}

		public async Task<IActionResult> DeleteOrder(int id)
		{
			if (id == 0 || _orderRep == null)
			{
				return NotFound();
			}

			var order = _orderRep.GetOrderById(id);
			if (order == null)
			{
				return NotFound();
			}
			ViewBag.OrdersPageID = GetCookie("OrdersPageNumber");
			return View(order);
		}

		[HttpPost, ActionName("DeleteOrder")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> OrderDeleteConfirmed(int id)
		{
			if (_orderRep == null)
			{
				return Problem("Entity set 'FasleSafarContext.Orders'  is null.");
			}
			_orderRep.RemoveOrder(id);
			return Redirect("/Admin/ShowOrders?pageid=" + GetCookie("OrdersPageNumber"));
		}

		public async Task<IActionResult> ReadOrder(int id)
		{
			if (id == 0 || _orderRep == null)
			{
				return NotFound();
			}

			var order = _orderRep.GetOrderById(id);
			if (order == null)
			{
				return NotFound();
			}
			ViewBag.OrdersPageID = GetCookie("OrdersPageNumber");
			return View(order);
		}

		#endregion

		#region ReqTrips

		public async Task<IActionResult> ShowReqTrips(int pageid = 1)
		{
			SetDefaultPageNumbers(10, pageid);
			int skip = (pageid - 1) * 20;
			int Count = _reqTripRep.GetAllReqTrips().Count();
			ViewBag.ReqTripsPageID = pageid;
			ViewBag.ReqTripsPageCount = Count / 20;
			return View(_reqTripRep.GetReqTripsForPages(skip));
		}
		public async Task<IActionResult> ShowUserReqTrips(int id, int pageid = 1)
		{
			SetCookie("CurUserId", id.ToString());
			ViewBag.UserId = id;
			ViewBag.FullName = _userRep.GetUserById(id).FullName;
			SetDefaultPageNumbers(11, pageid);
			int skip = (pageid - 1) * 20;
			int Count = _reqTripRep.GetReqTripsByUserId(id).Count;
			ViewBag.UserReqTripsPageID = pageid;
			ViewBag.UserReqTripsPageCount = Count / 20;
			return View(_reqTripRep.GetReqTripsOfUserForPages(id, skip));
		}

		public async Task<IActionResult> DeleteReqTrip(int id)
		{
			if (id == 0 || _reqTripRep == null)
			{
				return NotFound();
			}

			var reqTrip = _reqTripRep.GetReqTripById(id);
			if (reqTrip == null)
			{
				return NotFound();
			}
			ViewBag.ReqTripsPageID = GetCookie("ReqTripsPageNumber");
			return View(reqTrip);
		}

		[HttpPost, ActionName("DeleteReqTrip")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ReqTripDeleteConfirmed(int id)
		{
			if (_reqTripRep == null)
			{
				return Problem("Entity set 'FasleSafarContext.ReqTrips'  is null.");
			}
			_reqTripRep.RemoveReqTrip(id);
			return Redirect("/Admin/ShowReqTrips?pageid=" + GetCookie("ReqTripsPageNumber"));
		}

		public async Task<IActionResult> ReadReqTrip(int id)
		{
			if (id == 0 || _reqTripRep == null)
			{
				return NotFound();
			}

			var reqTrip = _reqTripRep.GetReqTripById(id);
			if (reqTrip == null)
			{
				return NotFound();
			}
			ViewBag.ReqTripsPageID = GetCookie("ReqTripsPageNumber");
			return View(reqTrip);
		}

		#endregion

		#region Messages

		public async Task<IActionResult> ShowMessages(int pageid = 1)
		{
			SetDefaultPageNumbers(12, pageid);
			int skip = (pageid - 1) * 20;
			int Count = _messageRep.GetAllMessages().Count();
			ViewBag.MessagesPageID = pageid;
			ViewBag.MessagesPageCount = Count / 20;
			return View(_messageRep.GetMessagesForPages(skip));
		}

		public async Task<IActionResult> ShowUserMessages(int id, int pageid = 1)
		{
			SetCookie("CurUserId", id.ToString());
			ViewBag.UserId = id;
			ViewBag.FullName = _userRep.GetUserById(id).FullName;
			SetDefaultPageNumbers(6, pageid);
			int skip = (pageid - 1) * 20;
			int Count = _messageRep.GetMessagesByUserId(id).Count;
			ViewBag.UserMessagesPageID = pageid;
			ViewBag.UserMessagesPageCount = Count / 20;
			return View(_messageRep.GetMessagesOfUserForPages(id, skip));
		}

		public IActionResult AddMessage(int id)
		{
			if (id == 0)
			{
				return NotFound();
			}
			ViewBag.ParticipantMob = _userRep.GetUserById(id).MobileNumber;
			ViewBag.ParticipantName = _userRep.GetUserById(id).FullName;
			ViewBag.MessagePageID = GetCookie("MessagesPageNumber");
			return View();
		}
		public IActionResult MakeMessage()
		{
			return View();
		}

		// POST: Admins/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddMessage(SendMessageVM messageVM)
		{
			ViewBag.MessagePageID = GetCookie("MessagesPageNumber");
			if (ModelState.IsValid)
			{
				bool sentState = await SendMessage(messageVM.MobileNumber, messageVM.MessageText);
				Message theMessage = new Message()
				{
					UserId = _userRep.GetUserByMobileNumber(messageVM.MobileNumber).UserId,
					MessageText = messageVM.MessageText,
					SentState = (sentState) ? "موفق" : "ناموفق",
					SentDate = DateTime.Now.ToString("yyyy/MM/dd - HH:mm").ToShamsi()
				};
				_messageRep.AddMessage(theMessage);
				return Redirect("/Admin/ShowMessages?pageid=" + GetCookie("MessagesPageNumber"));
			}
			return View(messageVM);
		}

		public async Task<IActionResult> DeleteMessage(int id)
		{
			if (id == 0 || _messageRep == null)
			{
				return NotFound();
			}

			var message = _messageRep.GetMessageById(id);
			if (message == null)
			{
				return NotFound();
			}
			ViewBag.MessagePageID = GetCookie("MessagesPageNumber");
			return View(message);
		}

		[HttpPost, ActionName("DeleteMessage")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> MessageDeleteConfirmed(int id)
		{
			if (_messageRep == null)
			{
				return Problem("Entity set 'SendReceiptContext.Receipts'  is null.");
			}
			_messageRep.RemoveMessage(id);
			return Redirect("/Admin/ShowMessages?pageid=" + GetCookie("MessagesPageNumber"));
		}

		public async Task<IActionResult> ReadMessage(int id)
		{
			if (id == 0 || _messageRep == null)
			{
				return NotFound();
			}

			var message = _messageRep.GetMessageById(id);
			if (message == null)
			{
				return NotFound();
			}
			ViewBag.MessagesPageID = GetCookie("MessagesPageNumber");
			return View(message);
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
		public IActionResult VerifyMobileNumber(string MobileNumber) // an Action that remoted for check the field value validation (no need to post page)
		{
			if (_userRep.ExistMobileNumber(MobileNumber.ToLower()))
			{
				return Json(true); // send true value
			}
			else return Json($"شماره موبایل {MobileNumber} در سیستم وجود ندارد"); //send error text
		}

		[HttpPost]
		public async Task<string> GetUserNameByMobileNumber(string mobileNumber)
		{
			if (mobileNumber == null) return null;
			if (_userRep.GetUserByMobileNumber(mobileNumber) == null) return null;
			return _userRep.GetUserByMobileNumber(mobileNumber).FullName;
		}

		#endregion

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

		private void SetDefaultPageNumbers(int field, int pageid)
		{
			switch (field)
			{
				case 1:
				default:
					SetCookie("DestinationsPageNumber", pageid.ToString());
					SetCookie("DestinationToursPageNumber", "1");
					SetCookie("ContentsPageNumber", "1");
					SetCookie("UsersPageNumber", "1");
					SetCookie("OrdersPageNumber", "1");
					SetCookie("OrdersofUserPageNumber", "1");
					SetCookie("UsersofTourPageNumber", "1");
					SetCookie("ToursofUserPageNumber", "1");
					SetCookie("ToursPageNumber", "1");
					SetCookie("ReqTripsPageNumber", "1");
					SetCookie("ReqTripsofUserPageNumber", "1");
					SetCookie("MessagesPageNumber", "1");
					SetCookie("MessagesofUserPageNumber", "1");
					SetCookie("AttractionsPageNumber", "1");
					SetCookie("DestinationAttractionsPageNumber", "1");
					break;
				case 2:
					SetCookie("DestinationsPageNumber", "1");
					SetCookie("DestinationToursPageNumber", pageid.ToString());
					SetCookie("ContentsPageNumber", "1");
					SetCookie("UsersPageNumber", "1");
					SetCookie("OrdersPageNumber", "1");
					SetCookie("OrdersofUserPageNumber", "1");
					SetCookie("UsersofTourPageNumber", "1");
					SetCookie("ToursofUserPageNumber", "1");
					SetCookie("ToursPageNumber", "1");
					SetCookie("ReqTripsPageNumber", "1");
					SetCookie("ReqTripsofUserPageNumber", "1");
					SetCookie("MessagesPageNumber", "1");
					SetCookie("MessagesofUserPageNumber", "1");
					SetCookie("AttractionsPageNumber", "1");
					SetCookie("DestinationAttractionsPageNumber", "1");
					break;
				case 3:
					SetCookie("DestinationsPageNumber", "1");
					SetCookie("DestinationToursPageNumber", "1");
					SetCookie("ContentsPageNumber", pageid.ToString());
					SetCookie("UsersPageNumber", "1");
					SetCookie("OrdersPageNumber", "1");
					SetCookie("OrdersofUserPageNumber", "1");
					SetCookie("UsersofTourPageNumber", "1");
					SetCookie("ToursofUserPageNumber", "1");
					SetCookie("ToursPageNumber", "1");
					SetCookie("ReqTripsPageNumber", "1");
					SetCookie("ReqTripsofUserPageNumber", "1");
					SetCookie("MessagesPageNumber", "1");
					SetCookie("MessagesofUserPageNumber", "1");
					SetCookie("AttractionsPageNumber", "1");
					SetCookie("DestinationAttractionsPageNumber", "1");
					break;
				case 4:
					SetCookie("DestinationsPageNumber", "1");
					SetCookie("DestinationToursPageNumber", "1");
					SetCookie("ContentsPageNumber", "1");
					SetCookie("UsersPageNumber", pageid.ToString());
					SetCookie("OrdersPageNumber", "1");
					SetCookie("OrdersofUserPageNumber", "1");
					SetCookie("UsersofTourPageNumber", "1");
					SetCookie("ToursofUserPageNumber", "1");
					SetCookie("ToursPageNumber", "1");
					SetCookie("ReqTripsPageNumber", "1");
					SetCookie("ReqTripsofUserPageNumber", "1");
					SetCookie("MessagesPageNumber", "1");
					SetCookie("MessagesofUserPageNumber", "1");
					SetCookie("AttractionsPageNumber", "1");
					SetCookie("DestinationAttractionsPageNumber", "1");
					break;
				case 5:
					SetCookie("DestinationsPageNumber", "1");
					SetCookie("DestinationToursPageNumber", "1");
					SetCookie("ContentsPageNumber", "1");
					SetCookie("UsersPageNumber", "1");
					SetCookie("OrdersPageNumber", pageid.ToString());
					SetCookie("OrdersofUserPageNumber", "1");
					SetCookie("UsersofTourPageNumber", "1");
					SetCookie("ToursofUserPageNumber", "1");
					SetCookie("ToursPageNumber", "1");
					SetCookie("ReqTripsPageNumber", "1");
					SetCookie("ReqTripsofUserPageNumber", "1");
					SetCookie("MessagesPageNumber", "1");
					SetCookie("MessagesofUserPageNumber", "1");
					SetCookie("AttractionsPageNumber", "1");
					SetCookie("DestinationAttractionsPageNumber", "1");
					break;
				case 6:
					SetCookie("DestinationsPageNumber", "1");
					SetCookie("DestinationToursPageNumber", "1");
					SetCookie("ContentsPageNumber", "1");
					SetCookie("UsersPageNumber", "1");
					SetCookie("OrdersPageNumber", "1");
					SetCookie("OrdersofUserPageNumber", pageid.ToString());
					SetCookie("UsersofTourPageNumber", "1");
					SetCookie("ToursofUserPageNumber", "1");
					SetCookie("ToursPageNumber", "1");
					SetCookie("ReqTripsPageNumber", "1");
					SetCookie("ReqTripsofUserPageNumber", "1");
					SetCookie("MessagesPageNumber", "1");
					SetCookie("MessagesofUserPageNumber", "1");
					SetCookie("AttractionsPageNumber", "1");
					SetCookie("DestinationAttractionsPageNumber", "1");
					break;
				case 7:
					SetCookie("DestinationsPageNumber", "1");
					SetCookie("DestinationToursPageNumber", "1");
					SetCookie("ContentsPageNumber", "1");
					SetCookie("UsersPageNumber", "1");
					SetCookie("OrdersPageNumber", "1");
					SetCookie("OrdersofUserPageNumber", "1");
					SetCookie("UsersofTourPageNumber", pageid.ToString());
					SetCookie("ToursofUserPageNumber", "1");
					SetCookie("ToursPageNumber", "1");
					SetCookie("ReqTripsPageNumber", "1");
					SetCookie("ReqTripsofUserPageNumber", "1");
					SetCookie("MessagesPageNumber", "1");
					SetCookie("MessagesofUserPageNumber", "1");
					SetCookie("AttractionsPageNumber", "1");
					SetCookie("DestinationAttractionsPageNumber", "1");
					break;
				case 8:
					SetCookie("DestinationsPageNumber", "1");
					SetCookie("DestinationToursPageNumber", "1");
					SetCookie("ContentsPageNumber", "1");
					SetCookie("UsersPageNumber", "1");
					SetCookie("OrdersPageNumber", "1");
					SetCookie("OrdersofUserPageNumber", "1");
					SetCookie("UsersofTourPageNumber", "1");
					SetCookie("ToursofUserPageNumber", pageid.ToString());
					SetCookie("ToursPageNumber", "1");
					SetCookie("ReqTripsPageNumber", "1");
					SetCookie("ReqTripsofUserPageNumber", "1");
					SetCookie("MessagesPageNumber", "1");
					SetCookie("MessagesofUserPageNumber", "1");
					SetCookie("AttractionsPageNumber", "1");
					SetCookie("DestinationAttractionsPageNumber", "1");
					break;
				case 9:
					SetCookie("DestinationsPageNumber", "1");
					SetCookie("DestinationToursPageNumber", "1");
					SetCookie("ContentsPageNumber", "1");
					SetCookie("UsersPageNumber", "1");
					SetCookie("OrdersPageNumber", "1");
					SetCookie("OrdersofUserPageNumber", "1");
					SetCookie("UsersofTourPageNumber", "1");
					SetCookie("ToursofUserPageNumber", "1");
					SetCookie("ToursPageNumber", pageid.ToString());
					SetCookie("ReqTripsPageNumber", "1");
					SetCookie("ReqTripsofUserPageNumber", "1");
					SetCookie("MessagesPageNumber", "1");
					SetCookie("MessagesofUserPageNumber", "1");
					SetCookie("AttractionsPageNumber", "1");
					SetCookie("DestinationAttractionsPageNumber", "1");
					break;
				case 10:
					SetCookie("DestinationsPageNumber", "1");
					SetCookie("DestinationToursPageNumber", "1");
					SetCookie("ContentsPageNumber", "1");
					SetCookie("UsersPageNumber", "1");
					SetCookie("OrdersPageNumber", "1");
					SetCookie("OrdersofUserPageNumber", "1");
					SetCookie("UsersofTourPageNumber", "1");
					SetCookie("ToursofUserPageNumber", "1");
					SetCookie("ToursPageNumber", "1");
					SetCookie("ReqTripsPageNumber", pageid.ToString());
					SetCookie("ReqTripsofUserPageNumber", "1");
					SetCookie("MessagesPageNumber", "1");
					SetCookie("MessagesofUserPageNumber", "1");
					SetCookie("AttractionsPageNumber", "1");
					SetCookie("DestinationAttractionsPageNumber", "1");
					break;
				case 11:
					SetCookie("DestinationsPageNumber", "1");
					SetCookie("DestinationToursPageNumber", "1");
					SetCookie("ContentsPageNumber", "1");
					SetCookie("UsersPageNumber", "1");
					SetCookie("OrdersPageNumber", "1");
					SetCookie("OrdersofUserPageNumber", "1");
					SetCookie("UsersofTourPageNumber", "1");
					SetCookie("ToursofUserPageNumber", "1");
					SetCookie("ToursPageNumber", "1");
					SetCookie("ReqTripsPageNumber", "1");
					SetCookie("ReqTripsofUserPageNumber", pageid.ToString());
					SetCookie("MessagesPageNumber", "1");
					SetCookie("MessagesofUserPageNumber", "1");
					SetCookie("AttractionsPageNumber", "1");
					SetCookie("DestinationAttractionsPageNumber", "1");
					break;
				case 12:
					SetCookie("DestinationsPageNumber", "1");
					SetCookie("DestinationToursPageNumber", "1");
					SetCookie("ContentsPageNumber", "1");
					SetCookie("UsersPageNumber", "1");
					SetCookie("OrdersPageNumber", "1");
					SetCookie("OrdersofUserPageNumber", "1");
					SetCookie("UsersofTourPageNumber", "1");
					SetCookie("ToursofUserPageNumber", "1");
					SetCookie("ToursPageNumber", "1");
					SetCookie("ReqTripsPageNumber", "1");
					SetCookie("ReqTripsofUserPageNumber", "1");
					SetCookie("MessagesPageNumber", pageid.ToString());
					SetCookie("MessagesofUserPageNumber", "1");
					SetCookie("AttractionsPageNumber", "1");
					SetCookie("DestinationAttractionsPageNumber", "1");
					break;
				case 13:
					SetCookie("DestinationsPageNumber", "1");
					SetCookie("DestinationToursPageNumber", "1");
					SetCookie("ContentsPageNumber", "1");
					SetCookie("UsersPageNumber", "1");
					SetCookie("OrdersPageNumber", "1");
					SetCookie("OrdersofUserPageNumber", "1");
					SetCookie("UsersofTourPageNumber", "1");
					SetCookie("ToursofUserPageNumber", "1");
					SetCookie("ToursPageNumber", "1");
					SetCookie("ReqTripsPageNumber", "1");
					SetCookie("ReqTripsofUserPageNumber", "1");
					SetCookie("MessagesPageNumber", "1");
					SetCookie("MessagesofUserPageNumber", pageid.ToString());
					SetCookie("AttractionsPageNumber", "1");
					SetCookie("DestinationAttractionsPageNumber", "1");
					break;
				case 14:
					SetCookie("DestinationsPageNumber", "1");
					SetCookie("DestinationToursPageNumber", "1");
					SetCookie("ContentsPageNumber", "1");
					SetCookie("UsersPageNumber", "1");
					SetCookie("OrdersPageNumber", "1");
					SetCookie("OrdersofUserPageNumber", "1");
					SetCookie("UsersofTourPageNumber", "1");
					SetCookie("ToursofUserPageNumber", "1");
					SetCookie("ToursPageNumber", "1");
					SetCookie("ReqTripsPageNumber", "1");
					SetCookie("ReqTripsofUserPageNumber", "1");
					SetCookie("MessagesPageNumber", "1");
					SetCookie("MessagesofUserPageNumber", "1");
					SetCookie("AttractionsPageNumber", pageid.ToString());
					SetCookie("DestinationAttractionsPageNumber", "1");
					break;
				case 15:
					SetCookie("DestinationsPageNumber", "1");
					SetCookie("DestinationToursPageNumber", "1");
					SetCookie("ContentsPageNumber", "1");
					SetCookie("UsersPageNumber", "1");
					SetCookie("OrdersPageNumber", "1");
					SetCookie("OrdersofUserPageNumber", "1");
					SetCookie("UsersofTourPageNumber", "1");
					SetCookie("ToursofUserPageNumber", "1");
					SetCookie("ToursPageNumber", "1");
					SetCookie("ReqTripsPageNumber", "1");
					SetCookie("ReqTripsofUserPageNumber", "1");
					SetCookie("MessagesPageNumber", "1");
					SetCookie("MessagesofUserPageNumber", "1");
					SetCookie("AttractionsPageNumber", "1");
					SetCookie("DestinationAttractionsPageNumber", pageid.ToString());
					break;
			}

		}


		#endregion

		#region ExportExcel

		public IActionResult ExportToExcel(int orderId = 0, int tourId = 0, int userId = 0)
		{
			decimal TotalSalePrice = 0;
			List<Order> orderList = new List<Order>();

			if (orderId > 0)
			{
				orderList.Add(_orderRep.GetOrderById(orderId));
			}
			else if (userId > 0)
			{
				orderList = _orderRep.GetPaidOrdersByUserId(userId);
			}
			else if (tourId > 0)
			{
				orderList = _orderRep.GetPaidOrdersByTourId(tourId);
			}
			else
			{
				orderList = _orderRep.GetAllPaidOrders();
			}

			var stream = new MemoryStream();
			string listtitle = "";
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
			using (var xlPackage = new ExcelPackage(stream))
			{
				var worksheet = xlPackage.Workbook.Worksheets.Add("Sheet1");
				var namedStyle = xlPackage.Workbook.Styles.CreateNamedStyle("HyperLink");
				namedStyle.Style.Font.UnderLine = true;
				namedStyle.Style.Font.Color.SetColor(Color.Blue);
				const int startRow = 2;
				var row = startRow;

				//Create Headers and format them
				worksheet.Cells["A1:O1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
				worksheet.Cells["A1:O1"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228));
				worksheet.Cells["A1:O1"].Style.Font.Bold = true;

				row = 2;

				listtitle = "Orders Document";

				worksheet.Cells["A1"].Value = "کد پیگیری سفارش";
				worksheet.Cells["B1"].Value = "عنوان تور";
				worksheet.Cells["C1"].Value = "نام خریدار";
				worksheet.Cells["D1"].Value = "نام مسافر";
				worksheet.Cells["E1"].Value = "نام خانوادگی مسافر";
				worksheet.Cells["F1"].Value = "رده سنی";
				worksheet.Cells["G1"].Value = "کد ملی";
				worksheet.Cells["H1"].Value = "شماره تلفن";
				worksheet.Cells["I1"].Value = "تاریخ تولد";
				worksheet.Cells["J1"].Value = "میزان تحصیلات";
				worksheet.Cells["K1"].Value = "شغل";
				worksheet.Cells["L1"].Value = "بیماری خاص";
				for (int i = 0; i < orderList.Count; i++)
				{
					TotalSalePrice += RetreivePrice(orderList[i].Price);
					var passengers = _passengerRep.GetPassengersOfOrder(orderList[i].OrderId);
					for (int j = 0; j < passengers.Count; j++)
					{
						worksheet.Cells[row, 1].Value = orderList[i].OrderId;
						worksheet.Cells[row, 2].Value = _tourRep.GetTourById(orderList[i].TourId.Value).TourTitle;
						worksheet.Cells[row, 3].Value = _userRep.GetUserById(orderList[i].UserId.Value).FullName;
						worksheet.Cells[row, 6].Value = passengers[j].AgeGroup;
						worksheet.Cells[row, 9].Value = passengers[j].BirthDate;
						worksheet.Cells[row, 12].Value = passengers[j].SpecialDisease;
						worksheet.Cells[row, 10].Value = passengers[j].EducationLevel;
						worksheet.Cells[row, 11].Value = passengers[j].Job;
						worksheet.Cells[row, 8].Value = passengers[j].PhoneNumber;
						worksheet.Cells[row, 7].Value = passengers[j].NationalCode;
						worksheet.Cells[row, 5].Value = passengers[j].LastName;
						worksheet.Cells[row, 4].Value = passengers[j].FirstName;
						row++;
					}
					worksheet.Cells[row, 1].Value = "----------------------------------";
					row++;
					worksheet.Cells[row, 1].Value = $"قیمت فاکتور:";
					worksheet.Cells[row, 2].Value = orderList[i].Price;
					row++;
				}
				row++;
				worksheet.Cells[row, 1].Value = "----------------------------------";
				row++;
				worksheet.Cells[row, 1].Value = $"مجموع فروش:";
				worksheet.Cells[row, 2].Value = TotalSalePrice.FixPrice();
				// set some core property values
				xlPackage.Workbook.Properties.Title = listtitle;
				xlPackage.Workbook.Properties.Author = User.Identity.Name;
				xlPackage.Workbook.Properties.Subject = listtitle;


				// save the new spreadsheet
				xlPackage.Save();
				// Response.Clear();
			}

			stream.Position = 0;
			return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{listtitle + "-" + DateTime.Now.ToString("yyyy/MM/dd - HH:mm").ToShamsi().Replace('/', '-').Replace(':', '_')}.xlsx");
		}

		#endregion

	}
}
