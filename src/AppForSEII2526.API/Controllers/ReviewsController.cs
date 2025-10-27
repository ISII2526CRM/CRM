using AppForSEII2526.API.DTOs.ReseñasDTOs;


namespace AppForSEII2526.API.Controllers
{
    // 2. Aplica los atributos de API directamente a la clase
    [Route("api/[controller]")] // [controller] se reemplazará por "GetDetailsReview"
    [ApiController]
    // 3. La clase hereda de ControllerBase y NO está anidada
    public class ReviewsController : ControllerBase
    {
        //used to enable your controller to access to the database
        private readonly ApplicationDbContext _context;
        //used to log any information when your system es running
        private readonly ILogger<ReviewsController> _logger; // <-- Cambiado a GetDetailsReviewController

        // 4. El constructor coincide con el nombre de la clase
        public ReviewsController(ApplicationDbContext context, ILogger<ReviewsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // 5. Tu método (¡que ya estaba bien!)
        [HttpGet]
        [Route("{id}")] // Ruta final: GET api/GetDetailsReview/5
        [ProducesResponseType(typeof(ReviewDetailsDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<ReviewDetailsDTO>> GetReviewDetails(int id)
        {
            // 1. Necesitamos incluir las relaciones para obtener los datos
            //    - Review.User (para el CustomerName)
            //    - Review.ReviewItems (para la lista)
            //    - Review.ReviewItems.Device (para los detalles del dispositivo)
            //    - Review.ReviewItems.Device.Model (basado en tu DTO anterior)
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