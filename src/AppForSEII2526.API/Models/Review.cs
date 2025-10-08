namespace AppForSEII2526.API.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        [StringLength(100)]
        [Required]
        public string ReviewTitle { get; set; }

        [Required]
        public string CustomerCountry { get; set; }

        // Opcional
        public string CustomerId { get; set; }

        [Required]
        public DateTime DateOfReview { get; set; } = DateTime.UtcNow;

        [Range(1, 5, ErrorMessage = "Overall rating must be between 1 and 5")]
        public int OverallRating { get; set; }

        // Relación uno a muchos con ReviewItem
        public IList<ReviewItem> ReviewItems { get; set; }

        // 🔹 Constructor vacío
        public Review()
        {
        }

        // 🔹 Constructor con todos los parámetros excepto ReviewId
        public Review(string reviewTitle, string customerCountry, string customerId, DateTime dateOfReview, int overallRating, IList<ReviewItem> reviewItems)
        {
            ReviewTitle = reviewTitle;
            CustomerCountry = customerCountry;
            CustomerId = customerId;
            DateOfReview = dateOfReview;
            OverallRating = overallRating;
            ReviewItems = reviewItems;
        }

        // 🔹 Constructor con ReviewId incluido
        public Review(int reviewId, string reviewTitle, string customerCountry, string customerId, DateTime dateOfReview, int overallRating, IList<ReviewItem> reviewItems)
            : this(reviewTitle, customerCountry, customerId, dateOfReview, overallRating, reviewItems)
        {
            ReviewId = reviewId;
        }
    }
}
