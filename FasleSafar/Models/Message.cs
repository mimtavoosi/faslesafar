using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FasleSafar.Models
{
    public class Message
    {
        [Key]
        [Display(Name = "کد پیامک")]
        public int MessageId { get; set; }

        [Display(Name = "کاربر")]
        public int? UserId { get; set; }

        [Display(Name = "متن پیامک")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(800)]
        public string MessageText { get; set; }

        [Display(Name = "وضعیت ارسال")]
        [MaxLength(800)]
        public string? SentState { get; set; }


        [Display(Name = "تاریخ ارسال")]
        [MaxLength(30)]
        public string? SentDate { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}