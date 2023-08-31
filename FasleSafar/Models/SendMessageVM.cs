using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FasleSafar.Models
{
    public class SendMessageVM
    {

        [Display(Name = "شماره موبایل")]
        [Required(ErrorMessage = "لطفا {0} شخص را وارد کنید")]
        [RegularExpression(@"^([0-9]{11})$", ErrorMessage = "مقدار {0} نامعتبر است")]
        [MaxLength(11)]
        [Remote("VerifyMobileNumber", "Admin")]
        public string MobileNumber { get; set; }

        [Display(Name = "متن پیامک")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(800)]
        public string MessageText { get; set; }
    }
}