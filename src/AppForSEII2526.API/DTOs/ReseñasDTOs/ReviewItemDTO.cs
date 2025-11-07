
namespace AppForSEII2526.API.DTOs.ReseñasDTOs
{
    public class ReviewItemDetailsDTO
    {
        
        public string DeviceName { get; set; }
        public string DeviceModel { get; set; }
        public int DeviceYear { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is ReviewItemDetailsDTO dTO &&
                   DeviceName == dTO.DeviceName &&
                   DeviceModel == dTO.DeviceModel &&
                   DeviceYear == dTO.DeviceYear &&
                   Rating == dTO.Rating &&
                   Comment == dTO.Comment;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(DeviceName, DeviceModel, DeviceYear, Rating, Comment);
        }
    }
}
