
namespace AppForSEII2526.API.DTOs.DeviceDTOs
{
	public class DeviceForRentalDTO
	{
		public DeviceForRentalDTO()
		{
		}
		public DeviceForRentalDTO(int id, string brand, string color, int year, string modelName, double priceForRent)
		{
			Id = id;
			Brand = brand;
			Color = color;
			Year = year;
			ModelName = modelName;
			PriceForRent = priceForRent;
		}

		public int Id { get; set; }
		public string Brand { get; set; }
		public string Color { get; set; }
		public int Year { get; set; }
		public string ModelName { get; set; }

        [Required]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Range(1, float.MaxValue, ErrorMessage = "El precio minimo es 1 ")]
        [Display(Name = "Price For Renting")]
        public double PriceForRent { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is DeviceForRentalDTO dTO &&
                   Id == dTO.Id &&
                   Brand == dTO.Brand &&
                   Color == dTO.Color &&
                   Year == dTO.Year &&
                   ModelName == dTO.ModelName &&
                   PriceForRent == dTO.PriceForRent;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Brand, Color, Year, ModelName, PriceForRent);
        }
    }
}
