﻿using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FasleSafar.Models
{
    public class ResetPasswordVM
	{
        [Required]
        public int UserId { get; set; }

        [Display(Name = "کلمه عبور جدید")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(20)]
        [DataType(DataType.Password)] //Hide Characters
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,20}$", ErrorMessage = "کلمه عبور باید شامل حرف و عدد باشد")] //check exist number & alphabet chars in password field
        public string NewPassword { get; set; }

        [Display(Name = "تکرار کلمه عبور جدید")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [DataType(DataType.Password)] //Hide Characters
        [Compare("NewPassword", ErrorMessage = "کلمه عبور و تکرار کلمه عبور یکسان نیستند")] //Compare value with Password field
        [MaxLength(20)]
        public string ReNewPassword { get; set; }

		[Required]
		public string Token { get; set; }
	}
}
