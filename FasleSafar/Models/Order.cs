using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FasleSafar.Models
{
    public class Order
    {
        [Key]
        [Display(Name = "کد سفارش")]
        public int OrderId { get; set; }

        [Required]
        [Display(Name = "خریدار")]
        public int? UserId { get; set; }

        [Required]
        [Display(Name = "تور")]
        public int? TourId { get; set; }

        [Display(Name = "قیمت")]
        public string? Price { get; set; }

        [Required]
        [Display(Name = "تعداد بزرگسال")]
        public int AdultCount { get; set; }
        [Required]
        [Display(Name = "تعداد کودک زیر 12 سال")]
        public int ChildCount { get; set; }

		[Display(Name = "تعداد کودک زیر 2 سال")]
		public int BabyCount { get; set; }

		[Required]
        [Display(Name = "تاریخ ثبت")]
        public string CreateDate { get; set; }

        [Display(Name = "وضعیت پرداخت")]
        public string IsFinaly { get; set; }

        [ForeignKey("UserId")] //the under relation is for UserID FK (one-one rel - one order is for one user)
        public User? User { get; set; }

        [ForeignKey("TourId")] //the under rel is for ProductId FK - this declare brcause ProductId FK is not the same name with Product PK (id)
        public Tour? Tour { get; set; } //one order detail include one order

		public List<Passenger>? Passengers { get; set; }
	}
}
