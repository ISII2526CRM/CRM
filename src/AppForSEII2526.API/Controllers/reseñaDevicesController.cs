using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using AppForSEII2526.API.DTOs;
using AppForSEII2526.API.DTOs.ReseñasDTOs;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class reseñaDevicesController : ControllerBase
    {
        //used to enable your controller to access to the database
        private readonly ApplicationDbContext _context;
        //used to log any information when your system is running
        private readonly ILogger<reseñaDevicesController> _logger;

        public reseñaDevicesController(ApplicationDbContext context, ILogger<reseñaDevicesController> logger)
        {
            _context = context;
            _logger = logger;
        }


        [HttpGet]
        [Route("action-filtrado")]
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
    }
}

