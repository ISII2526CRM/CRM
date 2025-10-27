namespace AppForSEII2526.API.DTOs.ReseñasDTOs
{
    public class ReviewItemDetailsDTO
    {
        public string DeviceName { get; set; }
        public string DeviceModel { get; set; }
        public int DeviceYear { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
