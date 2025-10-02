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

		public Purchase(int purchaseId, string customerUserName, string customerUserSurname, string deliveryAddress, PaymentMethodType paymentMethod, DateTime purchaseDate, double totalPrice, int totalQuantity) : 
			this(customerUserName, customerUserSurname, deliveryAddress, paymentMethod, purchaseDate, totalPrice, totalQuantity)
		{
			Id = purchaseId;
		}

		public Purchase(string customerUserName, string customerUserSurname, string deliveryAddress, PaymentMethodType paymentMethod, DateTime purchaseDate, double totalPrice, int totalQuantity)
		{
			CustomerUserName = customerUserName;
			CustomerUserSurname = customerUserSurname;
			DeliveryAddress = deliveryAddress;
			PaymentMethod = paymentMethod;
			PurchaseDate = purchaseDate;
			TotalPrice = totalPrice;
			TotalQuantity = totalQuantity;
		}

		public int Id { get; set; }

		public string CustomerUserName { get; set; }

		public string CustomerUserSurname { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.MultilineText)]
        [Display(Name = "Delivery Address")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please, set your address for delivery")]
        public string DeliveryAddress { get; set; }

        [Display(Name = "Payment Method")]
        public PaymentMethodTypes PaymentMethod { get; set; }

		public DateTime PurchaseDate { get; set; }

		public double TotalPrice { get; set; }

		public int TotalQuantity { get; set; }

	}
}
