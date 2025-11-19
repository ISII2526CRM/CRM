namespace AppForSEII2526.API.DTOs.RentalDTOs
{
    public class RentalForCreateDTO
    {
        public RentalForCreateDTO() { }
        public RentalForCreateDTO(string name, string surname, string deliveryAddress, PaymentMethodType paymentMethod, int quantity)
        {
            Name = name;
            Surname = surname;
            DeliveryAddress = deliveryAddress;
            PaymentMethod = paymentMethod;
            Quantity = quantity;
        }

        [Required(ErrorMessage = "El nombre del alquiler es obligatorio.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "El apellido del alquiler es obligatorio.")]
        public string Surname { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.MultilineText)]
        [Display(Name = "Delivery Address")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "La direccion de entrega debe tener al menos 10 caracteres.")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, indica tu direccion de entrega.")]
        public string DeliveryAddress { get; set; }

        [Required]
        public PaymentMethodType PaymentMethod { get; set; }

        //Cantidad mayor a 0
        [Required(ErrorMessage = "La cantidad del alquiler es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        public int Quantity { get; set; }

        //[Required]
        //[MinLength(1, ErrorMessage = "Se requiere al menos un dispositivo para el alquiler.")]
        //public List<RentDevicePostDTO> RentalItems { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is RentalForCreateDTO dTO &&
                   Name == dTO.Name &&
                   Surname == dTO.Surname &&
                   DeliveryAddress == dTO.DeliveryAddress &&
                   PaymentMethod == dTO.PaymentMethod &&
                   Quantity == dTO.Quantity;
            //RentalItems.SequenceEqual(dTO.RentalItems);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Surname, DeliveryAddress, PaymentMethod, Quantity);
        }

    }
}
