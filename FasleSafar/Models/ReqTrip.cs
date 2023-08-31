using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FasleSafar.Models
{
    public class ReqTrip
    {
        [Key]
        [Display(Name = "کد درخواست")]
        public int ReqTripId { get; set; }

        [Display(Name = "مقصد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(100)]
        public string DestinationName { get; set; }

        [Display(Name = "تاریخ رفت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(30)]
        public string StartDate { get; set; }

        [Display(Name = "تاریخ برگشت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(30)]
        public string EndDate { get; set; }

        [Display(Name = "تعداد مسافر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int PassengersCount { get; set; }

        [Display(Name = "شرح درخواست")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string ReqTripDescription { get; set; }

        [Display(Name = "نوع حمل و نقل")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(100)]
        public string TransportType { get; set; }

        [Display(Name = "تاریخ ثبت")]
        public string? CreateDate { get; set; }

        [Display(Name = "کاربر")]
        public int? UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}
