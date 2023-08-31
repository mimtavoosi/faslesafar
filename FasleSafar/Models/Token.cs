using System.ComponentModel.DataAnnotations;

namespace FasleSafar.Models
{
	public class Token
	{
		[Key]
		public int TokenId { get; set; }

		[Required]
		public string TokenText { get; set; }

		[Required]
		public DateTime TokenDateTime { get; set; }

		[Required]
		[MaxLength(50)]
		public string TokenType { get; set; }
		public bool TokenState { get; set; }
	}
}
