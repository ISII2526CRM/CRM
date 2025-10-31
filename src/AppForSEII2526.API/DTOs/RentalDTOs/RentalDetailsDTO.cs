namespace AppForSEII2526.API.DTOs.RentalDTOs
{
    public class RentalDetailsDTO
    {
        public RentalDetailsDTO() { }

        public RentalDetailsDTO(int id, string customerName, string customerSurname, string deliveryAddress, DateTime rentalDate, decimal totalPrice, int rentalPeriodDays, List<RentalItemDTO> rentalItems)
        {
            Id = id;
            RentalDate = rentalDate;
            CustomerName = customerName;
            CustomerSurname = customerSurname;
            DeliveryAddress = deliveryAddress;
            TotalPrice = totalPrice;
            RentalPeriodDays = rentalPeriodDays;
            RentalItems = rentalItems;
        }

        public int Id { get; set; }
        public DateTime RentalDate { get; set; }
        public string CustomerName { get; set; } // Agregado para corregir CS0117
        public string CustomerSurname { get; set; } // Agregado para el mapeo en RentalsController
        public string DeliveryAddress { get; set; } // Agregado para el mapeo en RentalsController
        public decimal TotalPrice { get; set; } // Agregado para el mapeo en RentalsController
        public int RentalPeriodDays { get; set; } // Agregado para el mapeo en RentalsController
        public List<RentalItemDTO> RentalItems { get; set; } // Agregado para el mapeo en RentalsController
    }
}