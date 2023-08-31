using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FasleSafar.Models
{
    public class AttractionVM
    {
        [Required]
        [Display(Name = "کد جادبه گردشگری")]
        public int AttractionId { get; set; }

        [Display(Name = "جادبه گردشگری")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(60)]
        public string AttractionName { get; set; }

        [Display(Name = "توضیحات")]
        public string? AttractionDescription { get; set; }

        [Display(Name = "تصویر بزرگ")]
        public string? BigImage { get; set; }

        [Display(Name = "آلبوم تصاویر")]
        public string? ImagesAlbum { get; set; }

        [Display(Name = "مختصات جغرافیایی")]
        public string? GeoCoordinates { get; set; }

        [Display(Name = "مقصد")]
        public int? DestinationId { get; set; }
    }
}
