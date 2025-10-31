
namespace AppForSEII2526.API.DTOs.ReseñasDTOs
{
    public class CreateReviewDTO
    {
        public CreateReviewDTO(string reviewTitle, string customerCountry, string? username)
        {
            ReviewTitle = reviewTitle;
            CustomerCountry = customerCountry;
            Username = username;
        }

        [Required(ErrorMessage = "El título de la reseña es obligatorio.")]
        public string ReviewTitle { get; set; } = string.Empty;

        // País obligatorio
        [Required(ErrorMessage = "El país desde donde se hace la reseña es obligatorio.")]
        public string CustomerCountry { get; set; } = string.Empty;

        // Nombre opcional
        public string? Username { get; set; }

        // Lista obligatoria de ítems (al menos 1)
        [Required(ErrorMessage = "Se requiere al menos un dispositivo en la reseña.")]
        [MinLength(1, ErrorMessage = "Se requiere al menos un dispositivo en la reseña.")]
        public List<ReviewItemDTO> ReviewItems { get; set; }

        public CreateReviewDTO()
        {
            ReviewItems = new List<ReviewItemDTO>();
        }

        public override bool Equals(object? obj)
        {
            return obj is CreateReviewDTO dTO &&
                   ReviewTitle == dTO.ReviewTitle &&
                   CustomerCountry == dTO.CustomerCountry &&
                   Username == dTO.Username &&
                   EqualityComparer<List<ReviewItemDTO>>.Default.Equals(ReviewItems, dTO.ReviewItems);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ReviewTitle, CustomerCountry, Username, ReviewItems);
        }
    }

    
}
