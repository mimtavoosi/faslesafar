using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FasleSafar.Models
{
    public class RatingHistory
    {
        [Key]
        public int RatingId { get; set; }
        public int? UserId { get; set; }
        public int? TourId { get; set; }
        public int Rate { get; set; }
    }
}
