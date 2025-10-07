namespace AppForSEII2526.API.Models
{
    [PrimaryKey(nameof(DeviceId), nameof(RentId))]
    public class RentDevice
    {
        public double Price { get; set; }

        public int Quantity { get; set; }

        // Foreign keys
        public IList<Device> Devices { get; set; }
        public int DeviceId { get; set; }
        
        public IList<Rental> Rentals { get; set; }
        public int RentId { get; set; }

    }
}
