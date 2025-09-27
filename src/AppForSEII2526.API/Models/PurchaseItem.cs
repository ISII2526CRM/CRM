using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
namespace AppForSEII2526.API.Models
{
	[PrimaryKey(nameof(DeviceId), nameof(PurchaseId))]
	public class PurchaseItem
	{
		public PurchaseItem()
		{
		}

		public PurchaseItem(string description, int deviceId, double price, int purchaseId, int quantity) : 
			this(description, price, quantity)
		{
			DeviceId = deviceId;
			PurchaseId = purchaseId;
		}
		public PurchaseItem(string description, double price, int quantity)
		{
			Description = description;
			Price = price;
			Quantity = quantity;
		}

		[AllowNull]
		public string Description { get; set; }

		public int DeviceId { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Range(0.5, float.MaxValue, ErrorMessage = "Minimum price is 0.5")]
        public double Price { get; set; }

		public int PurchaseId { get; set; }

		[Range(1, int.MaxValue, ErrorMessage = "You must provide a quantity higher than 1")]
		public int Quantity { get; set; }

	}
}
