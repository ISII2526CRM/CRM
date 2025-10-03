using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppForSEII2526.API.Models
{
    public class ReviewItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [StringLength(500, ErrorMessage = "Comment cannot exceed 500 characters.")]
        [Required]
        public string Comments { get; set; }

        // Relación con Device
        [ForeignKey("Device")]
        public int DeviceId { get; set; }
        public Device Device { get; set; }

        // Relación con Review
        [ForeignKey("Review")]
        public int ReviewId { get; set; }
        public Review Review { get; set; }
    }
}
