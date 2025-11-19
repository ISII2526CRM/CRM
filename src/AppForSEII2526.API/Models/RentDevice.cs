namespace AppForSEII2526.API.Models
{
    [PrimaryKey(nameof(DeviceId), nameof(RentalId))]
    public class RentDevice
    {
        public RentDevice() 
        { 
        }

        public RentDevice(int deviceId, int rentId)
        {
            Price = Device.PriceForRent;
            Quantity = Device.QuantityForRent;
            DeviceId = deviceId;
            RentalId = rentId;
        }

        public double Price { get; set; }

        public int Quantity { get; set; }

        // Foreign keys
        public Device Device { get; set; }
        public int DeviceId { get; set; }

        public Rental Rental { get; set; }
        public int RentalId { get; set; }

    }
}
