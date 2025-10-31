using AppForSEII2526.API.DTOs.RentalDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
                _logger.LogError("Error: La tabla Rental no existe");
                return NotFound();
            }

            var rentalDetails = await _context.Rental
                .Include(r => r.RentalItems)
                    .ThenInclude(ri => ri.Device)
                        .ThenInclude(d => d.Model)
                .Where(r => r.RentalId == id)
                .Select(r => new RentalDetailsDTO
                {
                    Id = r.RentalId,
                    CustomerName = r.CustomerName,
                    CustomerSurname = r.CustomerSurname,
                    DeliveryAddress = r.DeliveryAddress,
                    RentalDate = r.RentalDate,
                    TotalPrice = r.TotalPrice,
                    RentalPeriodDays = r.RentalPeriodDays,
                    RentalItems = r.RentalItems.Select(item => new RentalItemDTO
                    {
                        DeviceModel = item.Device.Model.NameModel,
                        PricePerDay = item.PricePerDay,
                        Quantity = item.Quantity
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (rentalDetails == null)
            {
                _logger.LogWarning($"No se encontró ningún alquiler con el ID {id}.");
                return NotFound($"No se encontró ningún alquiler con el ID {id}.");
            }
            return Ok(rentalDetails);
        }


    }
}