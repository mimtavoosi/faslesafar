using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FasleSafar.Models
{
    public class ForgotPasswordVM
    {
        [Required(ErrorMessage = "لطفا {0} را وارد کنید ")]
        [MaxLength(200)]
		[RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "پست الکترونیک معتبر نیست")]
		[Display(Name = "پست الکترونیک")]
        [Remote("ExistEmailAddress", "Account")]
        public string Email { get; set; }
    }
}
