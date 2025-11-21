using AppForSEII2526.API.DTOs.RentalDTOs;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        //used to enable your controller to access to the database
        private readonly ApplicationDbContext _context;
        //used to log any information when your system es running
        private readonly ILogger<RentalsController> _logger;

        public RentalsController(ApplicationDbContext context, ILogger<RentalsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(RentalDetailsDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetRentalDetails(int id)
        {
            if (_context.Rental == null)
            {
                _logger.LogError("Error: La tabla rental no existe.");
                return NotFound();
            }

            var rental = await _context.Rental
                .Include(r => r.User)
                .Include(r => r.RentDevices)
                    .ThenInclude(rd => rd.Device)
                        .ThenInclude(d => d.Model)
                .Where(r => r.Id == id)

                .Select(r => new RentalDetailsDTO(r.Id, r.User.Name, r.User.Surname,
                    r.DeliveryAddress, r.RentalDate,
                    r.RentalDateFrom, r.RentalDateTo,
                    r.RentDevices
                        .Select(rd => new RentDeviceDTO(rd.Device.Model.NameModel,
                                rd.Device.PriceForRent, rd.Device.QuantityForRent)).ToList<RentDeviceDTO>()))
                .FirstOrDefaultAsync();

            if (rental == null)
            {
                _logger.LogError($"No se encontró ningún alquiler con el ID {id}.");
                return NotFound();
            }

            return Ok(rental);
        }

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(RentalDetailsDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult> CreateRental(RentalForCreateDTO rentalForCreate)
        {
            //Con el RentDevicePostDTO

            //el payment method solo puede ser CreditCard o PayPal
            if (rentalForCreate.PaymentMethod != PaymentMethodType.CreditCard &&
                rentalForCreate.PaymentMethod != PaymentMethodType.Paypal)
            {
                ModelState.AddModelError("PaymentMethod", "Error! El método de pago debe ser 'CreditCard' o 'PayPal'.");
            }

            if (rentalForCreate.Quantity <= 0)
            {
                ModelState.AddModelError("Quantity", "Error! Debes alquilar al menos un dispositivo.");
            }

            //el nombre y apellido deban de coincidir con algun usuario registrado
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Name == rentalForCreate.Name && u.Surname == rentalForCreate.Surname);
            if (user == null)
                {
                ModelState.AddModelError("User", "Error! No se encontró ningún usuario con el nombre y apellido proporcionados.");
            }

            //string.IsNullOrWhiteSpace(rentalForCreate.DeliveryAddress)
            //la direccion de entrega no puede estar vacia
            if (!(rentalForCreate.DeliveryAddress == null) && !(rentalForCreate.DeliveryAddress.Contains("Calle")) && !(rentalForCreate.DeliveryAddress.Contains("Carretera")))
            {
                ModelState.AddModelError("DeliveryAddress", "Error en la dirección de envío. Por favor, introduce una dirección válida incluyendo las palabras Calle o Carretera");
            }

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

            //Crear el rental
            var rental = new Rental
            {
                DeliveryAddress = rentalForCreate.DeliveryAddress,
                PaymentMethod = rentalForCreate.PaymentMethod,
                RentalDate = DateTime.Today,
                RentalDateFrom = DateTime.Today,
                RentalDateTo = DateTime.Today.AddDays(7), //rental period of 7 days
                UserId = user!.Id
            };
            //Crear los RentDevice
            var rentDevices = new List<RentDevice>();
            // SOLUCIÓN: usar QuantityForRent en lugar de una propiedad inexistente IsAvailableForRent
            var availableDevices = await _context.Device
                .Include(d => d.Model)
                .Where(d => d.QuantityForRent > 0)
                .ToListAsync();
            if (availableDevices.Count < rentalForCreate.Quantity)
            {
                return Conflict("No hay suficientes dispositivos disponibles para alquilar la cantidad solicitada.");
            }
            for (int i = 0; i < rentalForCreate.Quantity; i++)
            {
                var device = availableDevices[i];
                var rentDevice = new RentDevice
                {
                    DeviceId = device.Id,
                    Price = device.PriceForRent,
                    Quantity = 1
                };
                rentDevices.Add(rentDevice);
                //Marcar la disminución de la cantidad disponible para alquilar en vez de usar una propiedad inexistente
                device.QuantityForRent = Math.Max(0, device.QuantityForRent - 1);
            }
            rental.RentDevices = rentDevices;
            //Guardar en la base de datos
            _context.Rental.Add(rental);
            await _context.SaveChangesAsync();

            // Volver a cargar el rental guardado con includes para tener navegación completa y poder mapear el DTO sin nulos
            var savedRental = await _context.Rental
                .Include(r => r.User)
                .Include(r => r.RentDevices)
                    .ThenInclude(rd => rd.Device)
                        .ThenInclude(d => d.Model)
                .FirstOrDefaultAsync(r => r.Id == rental.Id);

            if (savedRental == null)
            {
                // Esto no debería ocurrir, pero en caso de fallo devolvemos conflicto genérico
                return Conflict("Error al recuperar el alquiler creado.");
            }

            var rentalDetailsDTO = new RentalDetailsDTO(
                savedRental.Id,
                savedRental.User?.Name ?? string.Empty,
                savedRental.User?.Surname ?? string.Empty,
                savedRental.DeliveryAddress,
                savedRental.RentalDate,
                savedRental.RentalDateFrom,
                savedRental.RentalDateTo,
                savedRental.RentDevices.Select(rd => new RentDeviceDTO(
                    rd.Device.Model.NameModel,
                    rd.Price,
                    rd.Quantity)).ToList()
            );
            return CreatedAtAction(nameof(GetRentalDetails), new { id = savedRental.Id }, rentalDetailsDTO);

        }
    }
}