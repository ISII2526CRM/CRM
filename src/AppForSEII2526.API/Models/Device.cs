using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
namespace AppForSEII2526.API.Models
{
	[Index(nameof(Name), IsUnique = true)]
	public class Device
	{
		public Device()
		{
		}

		public Device(int id, Model model, string name, string brand, string color, double priceForPurchase, double priceForRent, 
			int quantityForPurchase, int quantityForRent, IList<PurchaseItem> purchaseItems, IList<RentDevice> rentedDevices, IList<ReviewItem> reviewItems, int year) : 
			this(model, name, brand, color, priceForPurchase, priceForRent, quantityForPurchase, quantityForRent, purchaseItems, rentedDevices ,reviewItems, year)
		{
			Id = id;
		}

		public Device(Model model, string name, string brand, string color, double priceForPurchase, double priceForRent, int quantityForPurchase, int quantityForRent, IList<PurchaseItem> purchaseItems, IList<RentDevice> rentedDevices, IList<ReviewItem> reviewItems, int year)
		{
			Model = model;
			Name = name; 
			Brand = brand; 
			Color = color;	
			PriceForPurchase = priceForPurchase; 
			PriceForRent = priceForRent;
			QuantityForPurchase = quantityForPurchase;
			QuantityForRent = quantityForRent;
			PurchaseItems = purchaseItems;
			RentedDevices = rentedDevices;
			ReviewItems = reviewItems;
			Year = year;
		}

		[Key]
		public int Id { get; set; }

		public Model Model { get; set; }

		[StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters.")]
		[Required]
		public string Name { get; set; }

		[StringLength(30, ErrorMessage = "Brand cannot be longer than 30 characters.")]
		[Required]
		public string Brand { get; set; }

		[StringLength(30, ErrorMessage = "Color cannot be longer than 30 characters.")]
		[AllowNull]
		public string Color { get; set; }

		[DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Range(0.5, float.MaxValue, ErrorMessage = "Minimum price is 0.5")]
        [Display(Name = "Price For Purchase")]
        public double PriceForPurchase { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
		[Range(0.5, float.MaxValue, ErrorMessage = "Minimum price is 0.5")]
		[Display(Name = "Price For Renting")]
		public double PriceForRent { get; set; }

		[Display(Name = "Quantity For Purchase")]
		[Range(0, int.MaxValue, ErrorMessage = "Minimum quantity for Purchase is 1")]
		public int QuantityForPurchase { get; set; }

		[Display(Name = "Quantity For Rent")]
		[Range(0, int.MaxValue, ErrorMessage = "Minimum quantity for Rent is 1")]
		public int QuantityForRent { get; set; }

		public IList<PurchaseItem> PurchaseItems { get; set; }
		public IList<RentDevice> RentedDevices { get; set; }

		public IList<ReviewItem> ReviewItems { get; set; }

		public int Year { get; set; }
    }
}
