using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FasleSafar.Models
{
	public class Passenger
	{

		[Key]
		public int PassengerId { get; set; }
		[MaxLength(50)]
		public string? AgeGroup { get; set; }
		[MaxLength(70)]
		public string? FirstName { get; set; }
		[MaxLength(100)]
		public string? LastName { get; set; }
		[MaxLength(10)]
		public string? NationalCode { get; set; }
		[MaxLength(11)]
		public string? PhoneNumber { get; set; }
		[MaxLength(30)]
		public string? BirthDate { get; set; }

		[MaxLength(100)]
		public string? EducationLevel { get; set; }
		[MaxLength(100)]
		public string? Job { get; set; }
		[MaxLength(200)]
		public string? SpecialDisease { get; set; }
		public int? OrderId { get; set; }

		[ForeignKey("OrderId")]
		public Order? Order { get; set; }
	}
}
