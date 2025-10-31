namespace AppForSEII2526.API.DTOs.ReseñasDTOs
{
    public class ReviewItemDTO
    {
        public ReviewItemDTO(string deviceName, string deviceModel, int deviceYear, int rating, string? comment)
        {
            DeviceName = deviceName;
            DeviceModel = deviceModel;
            DeviceYear = deviceYear;
            Rating = rating;
            Comment = comment;
        }

        public string DeviceName { get; set; }
        public string DeviceModel { get; set; }
        public int DeviceYear { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
