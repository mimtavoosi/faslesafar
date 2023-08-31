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
        [MaxLength(100)]
        public string? Title { get; set; }

        [Display(Name = "قیمت")]
        public decimal? Price { get; set; }

        [ForeignKey("TourId")]
        public Tour? Tour { get; set; }
    }
}
