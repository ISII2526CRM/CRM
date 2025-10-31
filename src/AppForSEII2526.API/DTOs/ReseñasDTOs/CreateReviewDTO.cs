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

        [Required]
        public string ReviewTitle { get; set; }

        [Required]
        public string CustomerCountry { get; set; }

        // Opcional: nombre de usuario del cliente (si existe en la base de datos)
        public string? Username { get; set; }

        
    }

    
}
