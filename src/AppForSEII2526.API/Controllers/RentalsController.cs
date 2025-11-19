using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        public async Task<ActionResult<RentalDetailsDTO>> GetRentalDetails(int id)
        {
            if (_context.Rental == null)
            {
                _logger.LogError("Error: La tabla rental no existe.");
                return NotFound();
            }
            var test = new Rental();

            var rental = await _context.Rental
                .Include(r => r.User)
                .Include(r => r.RentDevices)
                    .ThenInclude(rd => rd.Device)
                        .ThenInclude(d => d.Model)
                .Where(r => r.Id == id)
                /*.Select(r => new RentalDetailsDTO
                {
                    Id = r.Id,
                    Name = r.User.Name,
                    Surname = r.User.Surname,
                    DeliveryAddress = r.DeliveryAddress,
                    RentalDate = r.RentalDate,
                    TotalPrice = (decimal)r.TotalPrice,
                    RentalPeriodDays = (r.RentalDateTo - r.RentalDateFrom).Days,
                    RentalItems = r.RentDevices.Select(rd => new RentDeviceDTO
                    {
                        DeviceModel = rd.Device.Model.NameModel,
                        PricePerDay = (decimal)rd.Price,
                        Quantity = rd.Quantity
                    }).ToList()
                })
                .FirstOrDefaultAsync();*/

                .Select(r => new RentalDetailsDTO(r.Id, r.User.Name, r.User.Surname,
                    r.DeliveryAddress, r.RentalDate,
                    r.RentalDateFrom, r.RentalDateTo,
                    r.RentDevices
                        .Select(rd => new RentDeviceDTO(rd.Device.Model.NameModel,
                                rd.Device.PriceForRent, rd.Quantity)).ToList<RentDeviceDTO>()))
                .FirstOrDefaultAsync();

            if (rental == null)
            {
                _logger.LogWarning($"No se encontró ningún alquiler con el ID {id}.");
                return NotFound($"No se encontró ningún alquiler con el ID {id}.");
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
            /*El sistema muestra los dispositivos alquilados indicando el modelo, la marca y el
            precio de alquiler, y solicita al cliente que introduzca el nombre, los apellidos, la
            dirección de entrega, el método de pago (tarjeta de crédito, PayPal, efectivo) y la
            cantidad, todos ellos campos obligatorios.*/

            //el payment method es un enum, se debe validar que el valor recibido es valido
            if (!Enum.IsDefined(typeof(PaymentMethodType), rentalForCreate.PaymentMethod))
            {
                ModelState.AddModelError("PaymentMethod", "Error! El método de pago no es válido.");
            }

            //if (rentalForCreate.RentalItems.Count == 0)
            //ModelState.AddModelError("RentalItems", "Error! Debes alquilar almenos un dispositivo para alquilar");

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

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));
            //Crear el rental
            var rental = new Rental
            {
                DeliveryAddress = rentalForCreate.DeliveryAddress,
                PaymentMethod = rentalForCreate.PaymentMethod,
                RentalDate = DateTime.UtcNow,
                RentalDateFrom = DateTime.UtcNow,
                RentalDateTo = DateTime.UtcNow.AddDays(7), //rental period of 7 days
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