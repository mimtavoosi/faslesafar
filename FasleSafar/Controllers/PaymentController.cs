using FasleSafar.Data.Repositories;
using FasleSafar.Models;
using FasleSafar.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;
using Parbad;
using Parbad.AspNetCore;
using Parbad.Gateway.Melli;
using System.Security.Claims;
using System.Text;

namespace FasleSafar.Controllers
{
    public class PaymentController : Controller
    {
		public ITourRep _tourRep;
		public IOrderRep _orderRep;
        public IUserRep _userRep;
		private readonly IConfiguration _configuration;
		private readonly IOnlinePayment _onlinePayment;

        public PaymentController(ITourRep tourRep, IOrderRep orderRep,IUserRep userRep,IOnlinePayment onlinePayment,IConfiguration configuration)
        {
			_tourRep = tourRep;
			_orderRep = orderRep;
            _userRep = userRep;

			_configuration = configuration;

            _onlinePayment = onlinePayment;
        }


        #region SadadMelli

        public async Task<IActionResult> Pay()
        {
			int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)); //get userid in claims
            var order = _orderRep.GetCurrentOrderByUserId(userId);

            if (order == null)
                return Redirect("/Card/FinishBuy?id=" + order.OrderId);

			var callbackUrl = Url.Action("Verify", "Payment", new { fsorderId = order.OrderId }, Request.Scheme);

			var result = await _onlinePayment.RequestAsync(invoice =>
            {
				var settings = _configuration.GetSection("Settings").Get<ConfigSettings>();

				invoice.SetCallbackUrl(callbackUrl)
                       .SetAmount(RetreivePrice(order.Price + "0"?? "0"))
                       .SetGateway(settings.PayGateWay);

                invoice.UseAutoIncrementTrackingNumber();
                
            });
            
            // Note: Save the result.TrackingNumber in your database.

            if (result.IsSucceed)
            {
                return result.GatewayTransporter.TransportToGateway();
            }

            return Redirect("/Card/FinishBuy?id=" + order.OrderId);
        }
        [HttpGet, HttpPost]
        public async Task<IActionResult> Verify(string paymentToken="",int fsorderId = 0)
        {
			Order order = new Order();
			string redirectUrl = "";
			try
			{
				order = _orderRep.GetOrderById(fsorderId);

				var invoice = await _onlinePayment.FetchAsync();

				// Check if the invoice is new or it's already processed before.
				if (invoice.Status != PaymentFetchResultStatus.ReadyForVerifying)
				{
					// You can also see if the invoice is already verified before.
					var isAlreadyVerified = invoice.IsAlreadyVerified;
					order.IsFinaly = "لغو شده";
					redirectUrl = "/Card/FinishBuy?id=" + order.OrderId;
					_orderRep.EditOrder(order);
					return Redirect(redirectUrl);
				}

				// This is an example of cancelling an invoice when you think that the payment process must be stopped.
				if (!Is_There_Still_TourCapacity_In_Shop(order))
				{
					var cancelResult = await _onlinePayment.CancelAsync(invoice, cancellationReason: "Sorry, We have no more capacity to sell.");

					order.IsFinaly = "لغو شده";
					redirectUrl = "/Card/FinishBuy?id=" + order.OrderId + "&message=" + $"متاسفانه ظرفیت تور مورد نظر کمتر از تعداد بلیط درخواستی است \n در صورت کسر وجه مبلغ پرداختی طی 48 ساعت آینده عودت داده می شود.";
					_orderRep.EditOrder(order);
					return Redirect(redirectUrl);

				}

				var verifyResult = await _onlinePayment.VerifyAsync(invoice);

				// Note: Save the verifyResult.TransactionCode in your database.

				if (verifyResult.Status == PaymentVerifyResultStatus.Succeed)
				{
					order.IsFinaly = order.IsFinaly.Contains("اقساطی") ? "پرداخت اقساطی" : "پرداخت نقدی";
					
					order.Tour.Capacity -= (order.AdultCount + order.ChildCount + order.BabyCount);
					if (order.Tour.Capacity <= 0) order.Tour.OpenState = "پایان یافته";
					_tourRep.EditTour(order.Tour);
					redirectUrl = "/Card/FinishBuy?id=" + order.OrderId + "&refId=" + verifyResult.TransactionCode;
					_orderRep.EditOrder(order);
					return Redirect(redirectUrl);
				}

				else

				{
					order.IsFinaly = "لغو شده";
					redirectUrl = "/Card/FinishBuy?id=" + order.OrderId;
					_orderRep.EditOrder(order);
					return Redirect(redirectUrl);
				}
			}
            catch (Exception ex)
            {
				ToolBox.SaveLog(ex.Message + '\n' + ex.InnerException?.Message);	
            }
			return Redirect(redirectUrl);
		}

        //[HttpGet]
        //public IActionResult Refund()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> Refund(RefundViewModel viewModel)
        //{
        //    var result = await _onlinePayment.RefundCompletelyAsync(viewModel.TrackingNumber);

        //    return View("RefundResult", result);
        //}

        private static bool Is_There_Still_TourCapacity_In_Shop(Order order)
        {
            if(order.Tour.Capacity>= (order.AdultCount + order.ChildCount + order.BabyCount))  return true;
            else return false;
        }

		#endregion

		private decimal RetreivePrice(string finalPrice)
		{
			for (int i = 0; i < finalPrice.Length; i++)
			{
				if (finalPrice[i] == ',') finalPrice.Remove(i, 1);
			}
			return decimal.Parse(finalPrice);
		}
	}
}