using AppForSEII2526.Web.API;

namespace AppForSEII2526.Web
{
    public class RentalStateContainer
    {
        //we create an instance of Rental when an instance of RentalStateContainer is created
        public RentalForCreateDTO Rental { get; private set; } = new RentalForCreateDTO();

        public List<RentDeviceDTO> RentalItems { get; private set; } = new List<RentDeviceDTO>();

        public DateTime RentalDateFrom { get; set; } = DateTime.Today;
        public DateTime RentalDateTo { get; set; } = DateTime.Today.AddDays(7);

        //public RentalDetailsDTO RentalDetails { get; set; }

        //we compute the TotalPrice of the movies we have selected for renting them
        public double TotalPrice
        {
            get
            {
                int numberOfDays = (RentalDateTo - RentalDateFrom).Days;
                double itemsTotal = RentalItems.Sum(ri => ri.PricePerDay * ri.Quantity);
                return numberOfDays * itemsTotal;
            }
        }

        public event Action? OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();



        public void AddDeviceToRental(DeviceForRentalDTO device)
        {
            //before adding a movie we checked whether it has been already added
            if (!RentalItems.Any(ri => ri.DeviceModel == device.ModelName))
                //we add it if it is not in the list
                RentalItems.Add(new RentDeviceDTO()
                {
                    DeviceModel = device.ModelName,
                    PricePerDay = device.PriceForRent,
                    Quantity = 1
                }
            );
            else
            {
                //if it is already in the list, we just increase the quantity by 1
                //var item = RentalItems.First(ri => ri.DeviceModel == device.ModelName);
                RentalItems.First(ri => ri.DeviceModel == device.ModelName).Quantity += 1;
            }

            NotifyStateChanged();

        }

        //to delete movies from the list of selected movies
        public void RemoveRentalItemToRent(RentDeviceDTO item)
        {
            RentalItems.Remove(item);
            NotifyStateChanged();

        }

        //we eliminate all the movies from the list
        public void ClearRentingCart()
        {
            RentalItems.Clear();
            NotifyStateChanged();

        }

        //we have already finished the process of renting, thus, we create a new Rental 
        public void RentalProcessed()
        {
            //we have finished the rental process so we create a new object without data
            Rental = new RentalForCreateDTO();
            RentalItems = new List<RentDeviceDTO>();
            RentalDateFrom = DateTime.Today;
            RentalDateTo = DateTime.Today.AddDays(7);
            NotifyStateChanged();
        }
    }
}