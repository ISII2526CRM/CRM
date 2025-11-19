using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using AppForSEII2526.API.DTOs;
using AppForSEII2526.API.DTOs.ReseñasDTOs;
using AppForSEII2526.API.DTOs.DeviceDTOs;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        //used to enable your controller to access to the database
        private readonly ApplicationDbContext _context;
        //used to log any information when your system is running
        private readonly ILogger<DevicesController> _logger;

        public DevicesController(ApplicationDbContext context, ILogger<DevicesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<DevicesReseñaDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetDeviceForReviewFiltro(string? Brand, int? Year)

        {
            var query = _context.Device.AsQueryable();

            if (!string.IsNullOrWhiteSpace(Brand))
            {
                var brandNormalized = Brand.Trim().ToLower();
                query = query.Where(d => d.Brand.ToLower() == brandNormalized);
            }

            if (Year.HasValue)
            {
                query = query.Where(d => d.Year == Year.Value);
            }

            var devices = await query // <-- Usar 'query' en lugar de '_context.Device'
            .Select(m => new DevicesReseñaDTO(
                m.Id,
                m.Brand,
                m.Color,
                m.Year,
                m.Model.NameModel
            ))
            .ToListAsync();
        return Ok(devices);
        }

        //El sistema muestra la lista de dispositivos, indicando el nombre, el modelo, la marca, el año, el color y el precio del alquiler.
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<DeviceForRentalDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetDevicesForRental(string? NameModel, double? priceForRental)
        {
            var devices = await _context.Device
                .Include(d => d.Model)
                .Include(d => d.RentDevices).ThenInclude(rd => rd.Rental)

                .Where(d => d.QuantityForRent > 0 &&
                (string.IsNullOrEmpty(NameModel) || d.Model.NameModel.Contains(NameModel)) &&
                (!priceForRental.HasValue || d.PriceForRent == priceForRental))

                .Select(d => new DeviceForRentalDTO(d.Name, d.Model.NameModel, d.Brand, d.Year, d.Color, d.PriceForRent))

                //si la QuantityForRent es menor o igual a 0, el dispositivo no se muestra en la lista.
                //.Where(d => d.QuantityForRent > 0 &&
                //(string.IsNullOrEmpty(NameModel) || d.Model.NameModel.ToLower() == NameModel.ToLower()) &&
                //(!priceForRental.HasValue || d.PriceForRent <= priceForRental.Value))

                /*.Select(m => new DeviceForRentalDTO(
                m.Name,
                m.Model.NameModel,
                m.Brand,
                m.Year,
                m.Color,
                m.PriceForRent
                ))*/
                .ToListAsync();
            return Ok(devices);
        }
    }
}