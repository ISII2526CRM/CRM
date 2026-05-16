using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.RentalDTOs;

namespace AppForSEII2526.UT.RentalsController_test
{
    public class CreateRental_test : AppForSEII25264SqliteUT
    {
        /*El sistema muestra los dispositivos alquilados indicando el modelo, la marca y el
precio de alquiler, y solicita al cliente que introduzca el nombre, los apellidos, la
dirección de entrega, el método de pago (tarjeta de crédito, PayPal, efectivo) y la
cantidad, todos ellos campos obligatorios.*/
        private const string _name = "Miguel";
        private const string _surname = "Moreno";
        private const string _deliveryAddress = "Calle El Quijote";
        private const PaymentMethodType _paymentMethod = PaymentMethodType.CreditCard;
        private const int _quantity = 2;


        public CreateRental_test()
        {
            ApplicationUser user = new ApplicationUser("1", _name, _surname, "miguel12", "miguelmoreno@gmail.com");

            var models = new List<Model>() {
                new Model {NameModel = "Telefono" },
                new Model {NameModel = "Ordenador" },
                new Model {NameModel = "Tablet" },
                new Model {NameModel = "Raton" }
            };

            var devices = new List<Device>()
            {
                new Device (models[0], "XPS 15", "Dell", "Plata", 1850.99, 120.00, 10, _quantity, 2023),
                new Device (models[1], "Galaxy S24 Ultra", "Samsung", "Negro Titanio", 1400.00, 90.50, 25, _quantity, 2024),
                new Device (models[2], "MX Keys S", "Logitech", "Grafito", 109.99, 15.00, 50, _quantity, 2023),
                new Device (models[3], "MX Master 3S", "Logitech", "Negro", 99.50, 10.00, 60, _quantity, 2022)
            };

            var rental = new Rental(_deliveryAddress, _paymentMethod, new DateTime(2011, 10, 20), new DateTime(2011, 10, 20), new DateTime(2011, 10, 27), new List<RentDevice>(), "1");
            rental.RentDevices.Add(new RentDevice(devices[0], rental));
            rental.RentDevices.Add(new RentDevice(devices[1], rental));


            _context.ApplicationUser.Add(user);
            _context.AddRange(models);
            _context.AddRange(devices);
            _context.Add(rental);
            _context.SaveChanges();
        }

        public static IEnumerable<object[]> TestCasesFor_CreateRental() {
            var rentalWithInvalidName = new RentalForCreateDTO("", _surname, _deliveryAddress, _paymentMethod, _quantity);

            var rentalWithoInvalidSurname = new RentalForCreateDTO(_name, "", _deliveryAddress, _paymentMethod, _quantity);

            var rentalWithInvalidDeliveryAddress = new RentalForCreateDTO(_name, _surname, "C/ Rosario", _paymentMethod, _quantity);

            var rentalInvalidPaymentMethod = new RentalForCreateDTO(_name, _surname, _deliveryAddress, PaymentMethodType.Cash, _quantity);

            //si es menor o igual a 0
            var rentalWithInvalidQuantity = new RentalForCreateDTO(_name, _surname, _deliveryAddress, _paymentMethod, 0);

            var allTest = new List<object[]> {
                new object[] { rentalWithInvalidName, "Error! No se encontró ningún usuario con el nombre y apellido proporcionados.", },
                new object[] { rentalWithoInvalidSurname, "Error! No se encontró ningún usuario con el nombre y apellido proporcionados.", },
                new object[] { rentalWithInvalidDeliveryAddress, "Error en la dirección de envío. Por favor, introduce una dirección válida incluyendo las palabras Calle o Carretera", },
                new object[] { rentalInvalidPaymentMethod, "Error! El método de pago debe ser 'CreditCard' o 'PayPal'.", },
                new object[] { rentalWithInvalidQuantity, "Error! Debes alquilar al menos un dispositivo.", },
            };

            return allTest;
        }

        [Theory]
        [MemberData(nameof(TestCasesFor_CreateRental))]
        public async Task CreateRental_ErrorTest(RentalForCreateDTO rentalDTO, string errorExpected) {
            //arrange
            var mock = new Mock<ILogger<RentalsController>>();
            ILogger<RentalsController> logger = mock.Object;
            RentalsController controller = new RentalsController(_context, logger);

            //act
            var result = await controller.CreateRental(rentalDTO);

            //assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);
            var errorActual = problemDetails.Errors.First().Value[0];

            Assert.StartsWith(errorExpected, errorActual);
        }

        [Fact]
        public async Task CreateRental_SuccessTest() {
            //arrange
            var mock = new Mock<ILogger<RentalsController>>();
            ILogger<RentalsController> logger = mock.Object;
            RentalsController controller = new RentalsController(_context, logger);

            var rentalDTO = new RentalForCreateDTO(_name, _surname, _deliveryAddress, _paymentMethod, _quantity);

            var expectedRentalDetailsDTO = new RentalDetailsDTO(2, _name, _surname, _deliveryAddress, DateTime.Today, DateTime.Today, DateTime.Today.AddDays(7), new List<RentDeviceDTO>()
            { new RentDeviceDTO("Telefono", 120.00, 1), new RentDeviceDTO("Ordenador", 90.50, 1) });

            //act
            var result = await controller.CreateRental(rentalDTO);
            
            //assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var rentalActual = Assert.IsType<RentalDetailsDTO>(createdResult.Value);

            Assert.Equal(expectedRentalDetailsDTO, rentalActual);
        }
    }
}
