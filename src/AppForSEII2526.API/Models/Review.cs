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

        // 🔹 Clave foránea al usuario que creó la review
        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        // 🔹 Propiedad de navegación al usuario
        public ApplicationUser? User { get; set; }

        [Required]
        public DateTime DateOfReview { get; set; } = DateTime.UtcNow;

        [Range(1, 5, ErrorMessage = "Overall rating must be between 1 y 5")]
        public int OverallRating { get; set; }

        // Relación uno a muchos con ReviewItem
        public IList<ReviewItem> ReviewItems { get; set; } = new List<ReviewItem>();

        // 🔹 Constructores
        public Review() { }

        public Review(string reviewTitle, string customerCountry, string userId, DateTime dateOfReview, int overallRating, IList<ReviewItem> reviewItems)
        {
            ReviewTitle = reviewTitle;
            CustomerCountry = customerCountry;
            UserId = userId;
            DateOfReview = dateOfReview;
            OverallRating = overallRating;
            ReviewItems = reviewItems;
        }

        public Review(int reviewId, string reviewTitle, string customerCountry, string userId, DateTime dateOfReview, int overallRating, IList<ReviewItem> reviewItems)
            : this(reviewTitle, customerCountry, userId, dateOfReview, overallRating, reviewItems)
        {
            ReviewId = reviewId;
        }
    }
}
