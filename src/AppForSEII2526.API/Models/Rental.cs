namespace AppForSEII2526.API.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Rental
    {
        // Constructor vacio
        public Rental()
        {
        }

        //Constructor con parametros sin id
        public Rental(string deliveryAddress, PaymentMethodType paymentMethod, DateTime rentalDate, DateTime rentalDateFrom, DateTime rentalDateTo, double totalPrice)
        {
            DeliveryAddress = deliveryAddress;
            PaymentMethod = paymentMethod;
            RentalDate = rentalDate;
            RentalDateFrom = rentalDateFrom;
            RentalDateTo = rentalDateTo;
            TotalPrice = totalPrice;
        }

        //Constructor con parametros con id
        public Rental(int id, string deliveryAddress, PaymentMethodType paymentMethod, DateTime rentalDate, DateTime rentalDateFrom, DateTime rentalDateTo, double totalPrice) : this(deliveryAddress, paymentMethod, rentalDate, rentalDateFrom, rentalDateTo, totalPrice)
        {
            Id = id;
        }

        [Required]
        [StringLength(100, ErrorMessage = "El campo DeliveryAddress no puede ser mayor a 100 caracteres ni menor de 1 caracter.", MinimumLength = 1)]
        public string DeliveryAddress { get; set; }

        public int Id { get; set; }

        //[Required]
        //[StringLength(50, ErrorMessage = "El campo Name no puede ser mayor a 50 caracteres ni menor de 1 caracter.", MinimumLength=1)]
        //public string Name { get; set; }

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

        //[Required]
        //[StringLength(50, ErrorMessage = "El campo Surname no puede ser mayor a 50 caracteres ni menor de 1 caracter.", MinimumLength = 1)]
        //public string Surname { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Precio Total")]
        [Precision(5,2)]
        public double TotalPrice { get; set; }

        //Relacion uno a muchos con RentDevice
        public RentDevice RentDevice { get; set; }

        //Relacion uno a muchos con ApplicationUser
        public ApplicationUser User { get; set; }
    }

    public enum PaymentMethodType
    {
        CreditCard,
        Paypal,
        Cash
    }
}
