using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
        [ProducesResponseType(typeof(IList<Device>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetDeviceForReview()
        {
            IList<Device> devices = await _context.Device
                .ToListAsync();
            return Ok(devices);
        }
        

    }
}

