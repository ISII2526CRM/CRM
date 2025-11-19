using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.DeviceDTOs;


namespace AppForSEII2526.UT.DevicesController_test
{
    public class GetDevicesForRental_test: AppForSEII25264SqliteUT
    {
        public GetDevicesForRental_test()        
        {
            var models = new List<Model>() {
                new Model {NameModel = "Telefono" },
                new Model {NameModel = "Ordenador" },
                new Model {NameModel = "Tablet" },
                new Model {NameModel = "Raton" }
            };

            var devices = new List<Device>()
            {
                new Device (models[0], "XPS 15", "Dell", "Plata", 1850.99, 120.00, 10, 3, 2023),
                new Device (models[1], "Galaxy S24 Ultra", "Samsung", "Negro Titanio", 1400.00, 90.50, 25, 10, 2024),
                new Device (models[2], "MX Keys S", "Logitech", "Grafito", 109.99, 15.00, 50, 20, 2023),
                new Device (models[3], "MX Master 3S", "Logitech", "Negro", 99.50, 10.00, 60, 30, 2022)
            };

            _context.AddRange(models);
            _context.AddRange(devices); // Debe ser AddRange, no Add
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetDevicesForRentalAllNull_test() {
            //arrange
            List<DeviceForRentalDTO> expectedDevices = new List<DeviceForRentalDTO>()
            {
                new DeviceForRentalDTO("XPS 15", "Telefono", "Dell",2023, "Plata", 120.0),
                new DeviceForRentalDTO("Galaxy S24 Ultra", "Ordenador", "Samsung",2024,"Negro Titanio", 90.50),
                new DeviceForRentalDTO("MX Keys S", "Tablet", "Logitech",2023,"Grafito", 15.00),
                new DeviceForRentalDTO("MX Master 3S", "Raton", "Logitech",2022,"Negro", 10.00)
            };

            var mock = new Mock<ILogger<DevicesController>>();
            ILogger<DevicesController> logger = mock.Object;
            DevicesController controller = new DevicesController(_context, logger);

            //act
            var result = await controller.GetDevicesForRental(null, null);

            //assert
            var okresult = Assert.IsType<OkObjectResult>(result);
            var devicesactualresult = Assert.IsType<List<DeviceForRentalDTO>>(okresult.Value);
            Assert.Equal(expectedDevices, devicesactualresult);
        }

        [Fact]
        public async Task GetDevicesForRentalOnlyModel_test()
        {
            //arrange
            List<DeviceForRentalDTO> expectedDevices = new List<DeviceForRentalDTO>()
            {
                new DeviceForRentalDTO("Galaxy S24 Ultra", "Ordenador", "Samsung",2024,"Negro Titanio", 90.50)
            };

            var mock = new Mock<ILogger<DevicesController>>();
            ILogger<DevicesController> logger = mock.Object;
            DevicesController controller = new DevicesController(_context, logger);

            //act
            var result = await controller.GetDevicesForRental("Ordenador", null);

            //assert
            var okresult = Assert.IsType<OkObjectResult>(result);
            var devicesactualresult = Assert.IsType<List<DeviceForRentalDTO>>(okresult.Value);
            Assert.Equal(expectedDevices, devicesactualresult);
        }

        [Fact]
        public async Task GetDevicesForRentalOnlyModelIncomplete_test()
        {
            //arrange
            List<DeviceForRentalDTO> expectedDevices = new List<DeviceForRentalDTO>()
            {
                new DeviceForRentalDTO("XPS 15", "Telefono", "Dell",2023, "Plata", 120.0),
                new DeviceForRentalDTO("Galaxy S24 Ultra", "Ordenador", "Samsung",2024,"Negro Titanio", 90.50),
                new DeviceForRentalDTO("MX Master 3S", "Raton", "Logitech",2022,"Negro", 10.00)
            };

            var mock = new Mock<ILogger<DevicesController>>();
            ILogger<DevicesController> logger = mock.Object;
            DevicesController controller = new DevicesController(_context, logger);

            //act
            var result = await controller.GetDevicesForRental("o", null);

            //assert
            var okresult = Assert.IsType<OkObjectResult>(result);
            var devicesactualresult = Assert.IsType<List<DeviceForRentalDTO>>(okresult.Value);
            Assert.Equal(expectedDevices, devicesactualresult);
        }

        [Fact]
        public async Task GetDevicesForRentalOnlyPrice_test()
        {
            //arrange
            List<DeviceForRentalDTO> expectedDevices = new List<DeviceForRentalDTO>()
            {
                new DeviceForRentalDTO("MX Master 3S", "Raton", "Logitech",2022,"Negro", 10.00)
            };

            var mock = new Mock<ILogger<DevicesController>>();
            ILogger<DevicesController> logger = mock.Object;
            DevicesController controller = new DevicesController(_context, logger);

            //act
            var result = await controller.GetDevicesForRental(null, 10.00);

            //assert
            var okresult = Assert.IsType<OkObjectResult>(result);
            var devicesactualresult = Assert.IsType<List<DeviceForRentalDTO>>(okresult.Value);
            Assert.Equal(expectedDevices, devicesactualresult);
        }

        [Fact]
        public async Task GetDevicesForRentalOnlyNegativePrice_test()
        {
            //arrange
            List<DeviceForRentalDTO> expectedDevices = new List<DeviceForRentalDTO>()
            {
            };

            var mock = new Mock<ILogger<DevicesController>>();
            ILogger<DevicesController> logger = mock.Object;
            DevicesController controller = new DevicesController(_context, logger);

            //act
            var result = await controller.GetDevicesForRental(null, -10.00);

            //assert
            var okresult = Assert.IsType<OkObjectResult>(result);
            var devicesactualresult = Assert.IsType<List<DeviceForRentalDTO>>(okresult.Value);
            Assert.Equal(expectedDevices, devicesactualresult);
        }

        [Fact]
        public async Task GetDevicesForRentalAllNotNull_test()
        {
            //arrange
            List<DeviceForRentalDTO> expectedDevices = new List<DeviceForRentalDTO>()
            {
                new DeviceForRentalDTO("XPS 15", "Telefono", "Dell",2023, "Plata", 120.0)
            };

            var mock = new Mock<ILogger<DevicesController>>();
            ILogger<DevicesController> logger = mock.Object;
            DevicesController controller = new DevicesController(_context, logger);

            //act
            var result = await controller.GetDevicesForRental("Telefono", 120.0);

            //assert
            var okresult = Assert.IsType<OkObjectResult>(result);
            var devicesactualresult = Assert.IsType<List<DeviceForRentalDTO>>(okresult.Value);
            Assert.Equal(expectedDevices, devicesactualresult);
        }

    }
}
