namespace AppForSEII2526.API.DTOs.DeviceDTOs
{
	public class DeviceForRentalDTO
	{
		public DeviceForRentalDTO()
		{
		}
		public DeviceForRentalDTO(string nombre, string modelName, string brand, int year, string color, double priceForRent)
		{
            Nombre = nombre;
            ModelName = modelName;
            Brand = brand;
            Year = year;
            Color = color;
			PriceForRent = priceForRent;
		}

		public string Nombre { get; set; }
        public string ModelName { get; set; }
        public string Brand { get; set; }
        public int Year { get; set; }
        public string Color { get; set; }
		
		

        [Required]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Range(1, double.MaxValue, ErrorMessage = "El precio minimo es 1 ")]
        [Display(Name = "Price For Renting")]
        public double PriceForRent { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is DeviceForRentalDTO dTO &&
                   Nombre == dTO.Nombre &&
                   ModelName == dTO.ModelName &&
                   Brand == dTO.Brand &&
                   Year == dTO.Year &&
                   Color == dTO.Color &&
                   PriceForRent == dTO.PriceForRent;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Nombre, ModelName, Brand, Year, Color, PriceForRent);
        }
    }
}
