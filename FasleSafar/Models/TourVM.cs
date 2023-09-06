using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FasleSafar.Models
{
    public class TourVM
    {
        [Required]
        [Display(Name = "کد تور")]
        public int TourId { get; set; }

        [Display(Name = "عنوان تور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(100)]
        public string TourTitle { get; set; }

        [Display(Name = "تاریخ رفت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(30)]
        public string StartDate { get; set; }

        [Display(Name = "تاریخ برگشت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(30)]
        public string EndDate { get; set; }

        [Display(Name = "مدت تور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int DaysCount { get; set; }

        [Display(Name = "ظرفیت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int Capacity { get; set; }

        [Display(Name = "شروع قیمت")]
        [MaxLength(15)]
        public string? Price { get; set; }

        [Display(Name = "وسیله نقلیه")]
        [MaxLength(60)]
        public string? Vehicle { get; set; }

        [Display(Name = "شرح تور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string TourDescription { get; set; }

        [Display(Name = "نوع حمل و نقل")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(30)]
        public string TransportType { get; set; }

        [Display(Name = "نوع تور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(30)]
        public string TourType { get; set; }

        [Display(Name = "امتیاز")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(0, 5, ErrorMessage ="مقدار {0} باید بین 0 تا 5 باشد")]
        public float AvgScore { get; set; }

        [Display(Name = "مجموع امتیاز")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(0, 5, ErrorMessage = "مقدار {0} باید بین 0 تا 5 باشد")]
        public float TotalScore { get; set; }

        [Display(Name = "تعداد امتیازدهی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int ScoreCount { get; set; }

        [Display(Name = "فروش اقساطی")]
        public bool IsLeasing { get; set; }

        [Display(Name = "مقصد")]
        public int? DestinationId { get; set; }

        [Display(Name = "وضعیت تور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(30)]
        public string OpenState { get; set; }

        [Display(Name = "تصویر بزرگ")]
        public string? BigImage { get; set; }

        [Display(Name = "تصویر کوچک")]
        public string? SmallImage { get; set; }

        [Display(Name = "آلبوم تصاویر")]
        public string? ImagesAlbum { get; set; }

        [Display(Name = "زمان رسیدن")]
        public string? ReachTime { get; set; }

        [Display(Name = "زمان بازگشت")]
        public string? ReturnTime { get; set; }

		[Display(Name = "مختصات جغرافیایی")]
		public string? GeoCoordinates { get; set; }

		[Display(Name = "هزینه های مشمول")]
        public string? IncludeCosts { get; set; }

        [Display(Name = "هزینه های مستثنی")]
        public string? ExcludeCosts { get; set; }

        [Display(Name = "خدمات تور")]
        public string? Facilities { get; set; }

        [Display(Name = "جاذبه های تور")]
        public string? Attractions { get; set; }

        [ForeignKey("DestinationId")]
        public Destination? Destination { get; set; }

        public List<HotelStaring>? HotelStarings { get; set; }

        public int? FrameCase { get; set; } = 2;
    }
}
