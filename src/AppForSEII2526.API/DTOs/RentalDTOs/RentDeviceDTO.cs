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

    public class RentDevicePostDTO
    {
        public RentDevicePostDTO() { }
        public RentDevicePostDTO(string deviceModel, string brand, double pricePerDay)
        {
            DeviceModel = deviceModel;
            Brand = brand;
            PricePerDay = pricePerDay;
        }
        public string DeviceModel { get; set; }
        public string Brand { get; set; }
        public double PricePerDay { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is RentDevicePostDTO dTO &&
                   DeviceModel == dTO.DeviceModel &&
                   Brand == dTO.Brand &&
                   PricePerDay == dTO.PricePerDay;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(DeviceModel, Brand, PricePerDay);
        }
    }
}
