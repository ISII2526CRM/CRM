namespace AppForSEII2526.API.DTOs.DeviceDTOs
{
	public class DeviceForRentalDTO
	{
		public DeviceForRentalDTO()
		{
		}
		public DeviceForRentalDTO(int id, string brand, string color, int year, string modelName, decimal priceForRent)
		{
			Id = id;
			Brand = brand;
			Color = color;
			Year = year;
			ModelName = modelName;
			PriceForRent = priceForRent;
		}

		public int Id { get; set; }
		public string Brand { get; set; } = string.Empty;
		public string Color { get; set; } = string.Empty;
		public int Year { get; set; }
		public string ModelName { get; set; } = string.Empty;
		public decimal PriceForRent { get; set; }
	}
}
