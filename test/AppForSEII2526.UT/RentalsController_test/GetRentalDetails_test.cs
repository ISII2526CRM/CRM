using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.RentalDTOs;

namespace AppForSEII2526.UT.RentalsController_test
{
    public class GetRentalDetails_test : AppForSEII25264SqliteUT
    {
        public GetRentalDetails_test()
        {
            ApplicationUser user = new ApplicationUser("1", "Miguel", "Moreno", "miguel12", "miguelmoreno@gmail.com");

            var models = new List<Model>() {
                new Model {NameModel = "Telefono" },
                new Model {NameModel = "Ordenador" }
            };

            var devices = new List<Device>()
            {
                new Device (models[0], "XPS 15", "Dell", "Plata", 1850.99, 120.00, 10, 3, 2023),
                new Device (models[1], "Galaxy S24 Ultra", "Samsung", "Negro Titanio", 1400.00, 90.50, 25, 10, 2024)
            };

            var rental = new Rental("Calle El Quijote", PaymentMethodType.CreditCard, new DateTime(2011, 10, 20), new DateTime(2011, 10, 20), new DateTime(2011, 10, 27), new List<RentDevice>(), "1");
            rental.RentDevices.Add(new RentDevice(devices[0], rental));
            rental.RentDevices.Add(new RentDevice(devices[1], rental));


            _context.ApplicationUser.Add(user);
            _context.AddRange(models);
            _context.AddRange(devices);
            _context.Add(rental);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetRentalDetailsNotFound_test() {
            //arrange
            var mock = new Mock<ILogger<RentalsController>>();
            ILogger<RentalsController> logger = mock.Object;
            RentalsController controller = new RentalsController(_context, logger);

            //act
            var result = await controller.GetRentalDetails(999);

            //assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetRentalDetailsFound_test() {
            //arrage
            var expectedRentDevices = new List<RentDeviceDTO>
            {
                new RentDeviceDTO("Telefono", 120.00, 3),
                new RentDeviceDTO("Ordenador", 90.50, 10)
            };

            var expectedRental = new RentalDetailsDTO(1, "Miguel", "Moreno", "Calle El Quijote", new DateTime(2011, 10, 20), new DateTime(2011, 10, 20), new DateTime(2011, 10, 27), expectedRentDevices);
            //expectedRental.RentalItems.Add(new RentDeviceDTO("Telefono", 120.00, 3));
            //expectedRental.RentalItems.Add(new RentDeviceDTO("Ordenador", 90.50, 10));

            var mock = new Mock<ILogger<RentalsController>>();
            ILogger<RentalsController> logger = mock.Object;
            RentalsController controller = new RentalsController(_context, logger);

            //act
            var result = await controller.GetRentalDetails(1);

            //assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var rentalDTOActual = Assert.IsType<RentalDetailsDTO>(okResult.Value);
            //var eq = expectedRental.Equals(rentalDTOActual);

            Assert.Equal(expectedRental, rentalDTOActual);
        }

    }
}