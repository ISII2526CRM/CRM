using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
namespace AppForSEII2526.API.Models
{

	public class Purchase
	{
		public Purchase()
		{
		}

		public Purchase(int purchaseId, string deliveryAddress, PaymentMethodTypes paymentMethod, DateTime purchaseDate, double totalPrice, int totalQuantity, IList<PurchaseItem> purchaseItems) : 
			this(deliveryAddress, paymentMethod, purchaseDate, totalPrice, totalQuantity, purchaseItems)
		{
			Id = purchaseId;
		}

		public Purchase(string deliveryAddress, PaymentMethodTypes paymentMethod, DateTime purchaseDate, double totalPrice, int totalQuantity, IList<PurchaseItem> purchaseItems)
		{
			DeliveryAddress = deliveryAddress;
			PaymentMethod = paymentMethod;
			PurchaseDate = purchaseDate;
			TotalPrice = totalPrice;
			TotalQuantity = totalQuantity;
			PurchaseItems = purchaseItems;
		}

		[Key]
		public int Id { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.MultilineText)]
        [Display(Name = "Delivery Address")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please, set your address for delivery")]
        public string DeliveryAddress { get; set; }

        [Display(Name = "Payment Method")]
		[Required]
        public PaymentMethodTypes PaymentMethod { get; set; }

		[Required]
		public DateTime PurchaseDate { get; set; }

		[Required]
		public double TotalPrice { get; set; }

		[Required]
		public int TotalQuantity { get; set; }

		[Required]
		public IList<PurchaseItem> PurchaseItems { get; set; }

	}
    public enum PaymentMethodTypes
    {
		CreditCard,
		Paypal,
		Cash
    }
}
