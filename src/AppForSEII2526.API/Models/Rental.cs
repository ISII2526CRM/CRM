namespace AppForSEII2526.API.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Rental
    {
        // Constructor vacio
        public Rental()
        {
        }

        public Rental(string deliveryAddress, PaymentMethodType paymentMethod, DateTime rentalDate, DateTime rentalDateFrom, DateTime rentalDateTo, IList<RentDevice> rentDevices, string? userId)
        {
            DeliveryAddress = deliveryAddress;
            PaymentMethod = paymentMethod;
            RentalDate = rentalDate;
            RentalDateFrom = rentalDateFrom;
            RentalDateTo = rentalDateTo;
            TotalPrice = rentDevices.Sum(rd => rd.Price * (rentalDateTo - rentalDateFrom).Days);
            RentDevices = rentDevices;
            UserId = userId;
        }

        [Required]
        [StringLength(100, ErrorMessage = "El campo DeliveryAddress no puede ser mayor a 100 caracteres ni menor de 1 caracter.", MinimumLength = 1)]
        public string DeliveryAddress { get; set; }

        public int Id { get; set; }

        [Required]
        public PaymentMethodType PaymentMethod { get; set; }

        [DataType(DataType.Date), Display(Name = "Rental Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime RentalDate { get; set; }

        [DataType(DataType.Date), Display(Name = "Rental Date From")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime RentalDateFrom { get; set; }

        [DataType(DataType.Date), Display(Name = "Rental Date To")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime RentalDateTo { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Precio Total")]
        [Precision(5,2)]
        public double TotalPrice { get; set; }

        //Relacion uno a muchos con RentDevice
        public IList<RentDevice> RentDevices { get; set; }

        [Required]
        public string? UserId { get; set; }

        //Relacion uno a muchos con ApplicationUser
        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }
    }

    public enum PaymentMethodType
    {
        CreditCard,
        Paypal,
        Cash
    }
}