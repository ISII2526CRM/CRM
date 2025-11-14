using System.Net;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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


    }
}