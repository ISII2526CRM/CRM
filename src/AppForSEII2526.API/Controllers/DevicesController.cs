using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using AppForSEII2526.API.DTOs;
using AppForSEII2526.API.DTOs.ReseñasDTOs;

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
        [Route("action")]
        [ProducesResponseType(typeof(IList<DevicesReseñaDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetDeviceForReview()
        {
            var devices = await _context.Device
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

