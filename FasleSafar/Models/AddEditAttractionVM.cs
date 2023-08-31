using System.ComponentModel.DataAnnotations;

namespace FasleSafar.Models
{
    public class AddAttractionVM
    {
        [Required]
        [Display(Name = "کد جادبه گردشگری")]
        public int AttractionId { get; set; }

        [Display(Name = "جادبه گردشگری")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(60)]
        public string AttractionName { get; set; }

        [Display(Name = "مقصد")]
        [Required(ErrorMessage = "لطفا {0} را انتخاب کنید")]
        public int DestinationId { get; set; }

        [Display(Name = "توضیحات")]
        public string? AttractionDescription { get; set; }

        [Display(Name = "مختصات جغرافیایی")]
        public string? GeoCoordinates { get; set; }

        [Display(Name = "تصویر (1920 * 600)")]
        [Required(ErrorMessage = "لطفا {0} تور را وارد کنید")]
        public IFormFile BigImage { get; set; }

        [Display(Name = "آلبوم تصاویر (600 * 1920)")]
        public List<IFormFile>? AlbumImages { get; set; }
    }

    public class EditAttractionVM
    {
        [Required]
        [Display(Name = "کد جادبه گردشگری")]
        public int AttractionId { get; set; }

        [Display(Name = "جادبه گردشگری")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(60)]
        public string AttractionName { get; set; }

        [Display(Name = "مقصد")]
        [Required(ErrorMessage = "لطفا {0} را انتخاب کنید")]
        public int DestinationId { get; set; }

        [Display(Name = "توضیحات")]
        public string? AttractionDescription { get; set; }

        [Display(Name = "مختصات جغرافیایی")]
        public string? GeoCoordinates { get; set; }

        [Display(Name = "تصویر بزرگ")]
        public string? BigImage { get; set; }

        [Display(Name = "آلبوم تصاویر")]
        public string? ImagesAlbum { get; set; }

        [Display(Name = "تصویر (1920 * 600)")]
        public IFormFile? BigAttractionImage { get; set; }

        [Display(Name = "آلبوم تصاویر (600 * 1920)")]
        public List<IFormFile>? AlbumImages { get; set; }
    }
}
