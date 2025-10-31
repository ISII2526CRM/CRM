
namespace AppForSEII2526.API.DTOs.ReseñasDTOs
{
    public class ReviewItemDTO
    {
        public ReviewItemDTO(int deviceId, int rating, string? comment)
        {
            DeviceId = deviceId;
            Rating = rating;
            Comment = comment;
        }

        public ReviewItemDTO(string deviceName, string deviceModel, int deviceYear, int rating, string? comment)
        {
            DeviceName = deviceName;
            DeviceModel = deviceModel;
            DeviceYear = deviceYear;
            Rating = rating;
            Comment = comment;
        }

        public int DeviceId { get; set; }

        public string DeviceName { get; set; }

        public string DeviceModel { get; set; }

        public int DeviceYear { get; set; }

        public int Rating { get; set; }

        public string? Comment { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is ReviewItemDTO dTO &&
                   DeviceId == dTO.DeviceId &&
                   DeviceName == dTO.DeviceName &&
                   DeviceModel == dTO.DeviceModel &&
                   DeviceYear == dTO.DeviceYear &&
                   Rating == dTO.Rating &&
                   Comment == dTO.Comment;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(DeviceId, DeviceName, DeviceModel, DeviceYear, Rating, Comment);
        }
    }
}
