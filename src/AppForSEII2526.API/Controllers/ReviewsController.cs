using AppForSEII2526.API.DTOs.ReseñasDTOs;
using AppForSEII2526.API.DTOs.ReseñasDTOs; // La que tú pusiste (¡Correcto!)
using Microsoft.AspNetCore.Mvc;           // Para [Route], [ApiController], ControllerBase, etc.
using Microsoft.EntityFrameworkCore;      // Para .Include, .ThenInclude, .Select, .FirstOrDefaultAsync
using Microsoft.Extensions.Logging;       // Para ILogger
using System.Net;                           // Para HttpStatusCode
using System.Threading.Tasks;             // Para Task, async, await
using System.Linq;                          // Para .Select, .ToList (aunque EFCore a veces lo infiere)


using AppForSEII2526.API.Data;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        //used to enable your controller to access to the database
        private readonly ApplicationDbContext _context;
        //used to log any information when your system es running
        private readonly ILogger<ReviewsController> _logger;

        public ReviewsController(ApplicationDbContext context, ILogger<ReviewsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(ReviewDetailsDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<ReviewDetailsDTO>> GetReviewDetails(int id)
        {
            var reviewDetails = await _context.Review
                .Include(r => r.User)
                .Include(r => r.ReviewItems)
                    .ThenInclude(ri => ri.Device)
                        .ThenInclude(d => d.Model)
                .Where(r => r.ReviewId == id)

                // 2. Proyectamos el resultado al DTO
                .Select(r => new ReviewDetailsDTO
                {
                    Username = r.User.UserName,
                    CustomerCountry = r.CustomerCountry,
                    ReviewTitle = r.ReviewTitle,
                    DateOfReview = r.DateOfReview,

                    // 3. Mapeamos la sub-lista de ítems
                    ReviewItems = r.ReviewItems.Select(item => new ReviewItemDetailsDTO
                    {
                        DeviceName = item.Device.Name,
                        DeviceModel = item.Device.Model.NameModel,
                        DeviceYear = item.Device.Year,
                        Rating = item.Rating, // 'Rating' viene de ReviewItem
                        Comment = item.Comments // 'Comments' viene de ReviewItem
                    }).ToList()
                })
                .FirstOrDefaultAsync(); // Usamos FirstOrDefaultAsync porque solo buscamos UNO

            if (reviewDetails == null)
            {
                _logger.LogWarning($"No se encontró ninguna reseña con el ID {id}.");
                return NotFound($"No se encontró ninguna reseña con el ID {id}.");
            }

            return Ok(reviewDetails);
        }
    }
}