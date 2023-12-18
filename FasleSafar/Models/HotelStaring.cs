using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FasleSafar.Models
{
    public class HotelStaring
    {
        [Key]
        public int StaringId { get; set; }

        [Required]
        [Display(Name = "کد تور")]
        public int? TourId { get; set; }

        [Display(Name = "عنوان")]
        public string? Title { get; set; }

        [Display(Name = "قیمت بزرگسال")]
        public string? AdultPrice { get; set; }

        [Display(Name = "قیمت کودک زیر 12 سال")]
        public string? ChildPrice { get; set; }

        [Display(Name = "قیمت کودک زیر 2 سال")]
        public string? BabyPrice { get; set; }

        [ForeignKey("TourId")]
        public Tour? Tour { get; set; }
    }
}
