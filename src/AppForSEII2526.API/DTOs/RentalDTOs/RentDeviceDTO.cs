
namespace AppForSEII2526.API.DTOs.RentalDTOs
{
    public class RentDeviceDTO
    {
        public RentDeviceDTO() { }
        public RentDeviceDTO(string deviceModel, double pricePerDay, int quantity)
        {
            DeviceModel = deviceModel;
            PricePerDay = pricePerDay;
            Quantity = quantity;
        }

        public string DeviceModel { get; set; }
        public double PricePerDay { get; set; }
        public int Quantity { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is RentDeviceDTO dTO &&
                   DeviceModel == dTO.DeviceModel &&
                   PricePerDay == dTO.PricePerDay &&
                   Quantity == dTO.Quantity;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(DeviceModel, PricePerDay, Quantity);
        }
    }
}
