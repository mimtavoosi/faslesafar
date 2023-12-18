using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FasleSafar.Models
{
	public class Passenger
	{

		[Key]
		public int PassengerId { get; set; }
		public string? AgeGroup { get; set; }
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public string? NationalCode { get; set; }
		public string? PhoneNumber { get; set; }
		public string? BirthDate { get; set; }

		public string? EducationLevel { get; set; }
		public string? Job { get; set; }
		public string? SpecialDisease { get; set; }
		public int  OrderId { get; set; }

		[ForeignKey("OrderId")]
		public Order Order { get; set; }
	}
}
