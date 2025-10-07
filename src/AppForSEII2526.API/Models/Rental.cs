namespace AppForSEII2526.API.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Rental
    {
        [Required]
        [StringLength(100, ErrorMessage = "El campo DeliveryAddress no puede ser mayor a 100 caracteres ni menor de 1 caracter.", MinimumLength = 1)]
        public string DeliveryAddress { get; set; }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "El campo Name no puede ser mayor a 50 caracteres ni menor de 1 caracter.", MinimumLength=1)]
        public string Name { get; set; }

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

        [Required]
        [StringLength(50, ErrorMessage = "El campo Surname no puede ser mayor a 50 caracteres ni menor de 1 caracter.", MinimumLength = 1)]
        public string Surname { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Precio Total")]
        [Precision(5,2)]
        public double TotalPrice { get; set; }

        //Relacion uno a muchos con RentDevice
        public Device rentDevice { get; set; }


    }
    public enum PaymentMethodType
    {
        CreditCard,
        Paypal,
        Cash
    }
}
