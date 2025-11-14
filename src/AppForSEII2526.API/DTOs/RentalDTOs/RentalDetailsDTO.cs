namespace AppForSEII2526.API.DTOs.RentalDTOs
{
    public class RentalDetailsDTO
    {
        public RentalDetailsDTO() { }

        public RentalDetailsDTO(int id, string name, string surname, string deliveryAddress, DateTime rentalDate, DateTime rentalDateFrom, DateTime rentalDateTo,List<RentDeviceDTO> rentalItems)
        {
            Id = id;
            Name = name;
            Surname = surname;
            DeliveryAddress = deliveryAddress;
            RentalDate = rentalDate;
            TotalPrice = rentalItems.Sum(rd => rd.PricePerDay * (rentalDateTo - rentalDateFrom).Days);
            RentalPeriodDays = (rentalDateTo - rentalDateFrom).Days;
            RentalItems = rentalItems;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string DeliveryAddress { get; set; }
        public DateTime RentalDate { get; set; }
        public double TotalPrice { get; set; }
        public int RentalPeriodDays { get; set; }
        public List<RentDeviceDTO> RentalItems { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is RentalDetailsDTO dTO &&
                   Id == dTO.Id &&
                   Name == dTO.Name &&
                   Surname == dTO.Surname &&
                   DeliveryAddress == dTO.DeliveryAddress &&
                   RentalDate == dTO.RentalDate &&
                   TotalPrice == dTO.TotalPrice &&
                   RentalPeriodDays == dTO.RentalPeriodDays &&
                   EqualityComparer<List<RentDeviceDTO>>.Default.Equals(RentalItems, dTO.RentalItems);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, Surname, DeliveryAddress, RentalDate, TotalPrice, RentalPeriodDays, RentalItems);
        }
    }
}