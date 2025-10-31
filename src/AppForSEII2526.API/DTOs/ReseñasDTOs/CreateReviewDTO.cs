namespace AppForSEII2526.API.DTOs.ReseñasDTOs
{
    public class CreateReviewDTO
    {
        [Required]
        public string ReviewTitle { get; set; }

        [Required]
        public string CustomerCountry { get; set; }

        // Opcional: nombre de usuario del cliente (si existe en la base de datos)
        public string? Username { get; set; }

        [Required]
        public List<CreateReviewItemDTO> Items { get; set; } = new();
    }

    public class CreateReviewItemDTO
    {
        [Required]
        public int DeviceId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        public string Comment { get; set; }
    }
}
