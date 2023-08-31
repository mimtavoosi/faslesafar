using Microsoft.AspNetCore.Mvc;

namespace FasleSafar.Models
{
    public class FactorPricesVM
    {
        public int ChildCount { get; set; }
		public int BabyCount { get; set; }

		[Remote("CheckCapacity", "Card", AdditionalFields = "ChildCount,BabyCount")]
        public int AdultCount { get; set; }
        public string? OnePrice { get; set; }
        public string? Maliat { get; set; }
        public string? TotalPrice { get; set; }
        public int TourId { get; set; }
        public int PriceId { get; set; }
        public string? FinPrice { get; set; }
    }
}
