using AppForSEII2526.API.Data;
using AppForSEII2526.API.DTOs.ReseñasDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        // used to enable your controller to access the database
        private readonly ApplicationDbContext _context;
        // used to log any information when your system is running
        private readonly ILogger<ReviewsController> _logger;

        public ReviewsController(ApplicationDbContext context, ILogger<ReviewsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(ReviewDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<ReviewDTO>> GetReviewDetails(int id)
        {
            var reviewDetails = await _context.Review
                .Include(r => r.User)
                .Include(r => r.ReviewItems)
                    .ThenInclude(ri => ri.Device)
                        .ThenInclude(d => d.Model)
                .Where(r => r.ReviewId == id)
                // 2. Proyectamos el resultado al DTO
                .Select(r => new ReviewDTO
                {
                    ReviewId = r.ReviewId, // <--- Importante: Asegúrate de tener esto en ReviewDTO
                    Username = r.User.UserName,
                    CustomerCountry = r.CustomerCountry,
                    ReviewTitle = r.ReviewTitle,
                    DateOfReview = r.DateOfReview,

                    // 3. Mapeamos la sub-lista de ítems
                    ReviewItems = r.ReviewItems.Select(item => new ReviewItemDTO
                    {
                        DeviceName = item.Device.Name,
                        DeviceModel = item.Device.Model.NameModel,
                        DeviceYear = item.Device.Year,
                        Rating = item.Rating,
                        Comment = item.Comments
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (reviewDetails == null)
            {
                _logger.LogWarning($"No se encontró ninguna reseña con el ID {id}.");
                return NotFound($"No se encontró ninguna reseña con el ID {id}.");
            }

            return Ok(reviewDetails);
        }

        [HttpPost]
        [Route("[action]Review")]
        [ProducesResponseType(typeof(ReviewDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult> Create([FromBody] CreateReviewDTO input)
        {
            // Validación básica de DTO
            if (input == null)
                return BadRequest("Cuerpo de petición vacío.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Comprueba que existen items
            if (input.ReviewItems == null || !input.ReviewItems.Any())
                return BadRequest("Se requiere al menos un dispositivo con comentario y puntuación.");

            // Validar que todos los ratings y comentarios estén presentes
            foreach (var it in input.ReviewItems)
            {
                if (it.Rating < 1 || it.Rating > 5)
                    return BadRequest("La puntuación debe estar entre 1 y 5.");
                if ((string.IsNullOrWhiteSpace(it.Comment)) || !(it.Comment.StartsWith("Reseña para")))
                    return BadRequest("El comentario es obligatorio para cada dispositivo. Error, el comentario de la reseña: debe empezar por Reseña para...");
            }

            // Verificar que los dispositivos existen en la BBDD
            var deviceIds = input.ReviewItems.Select(i => i.DeviceId).Distinct().ToList();
            var devices = await _context.Device.Where(d => deviceIds.Contains(d.Id)).ToListAsync();
            var missing = deviceIds.Except(devices.Select(d => d.Id)).ToList();
            if (missing.Any())
                return BadRequest($"No se encontraron dispositivos con Ids: {string.Join(", ", missing)}");

            AppForSEII2526.API.Models.ApplicationUser? user = null;
            if (!string.IsNullOrWhiteSpace(input.Username))
            {
                user = await _context.ApplicationUser.FirstOrDefaultAsync(u => u.UserName == input.Username);
                if (user == null)
                {
                    return BadRequest($"Usuario '{input.Username}' no existe.");
                }
            }

            // Crear entidad Review
            var review = new AppForSEII2526.API.Models.Review
            {
                ReviewTitle = input.ReviewTitle,
                CustomerCountry = input.CustomerCountry,
                DateOfReview = DateTime.UtcNow,
                OverallRating = (int)Math.Round(input.ReviewItems.Average(i => i.Rating)),
                ReviewItems = new List<AppForSEII2526.API.Models.ReviewItem>()
            };

            if (user != null)
            {
                review.User = user;
            }

            // Crear ReviewItems y asociarlos
            foreach (var it in input.ReviewItems)
            {
                var reviewItem = new AppForSEII2526.API.Models.ReviewItem
                {
                    Device = devices.First(d => d.Id == it.DeviceId),
                    DeviceId = it.DeviceId,
                    Rating = it.Rating,
                    Comments = it.Comment,
                    Review = review
                };
                review.ReviewItems.Add(reviewItem);
            }

            // Guardar en BD
            _context.Review.Add(review);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error al guardar la reseña en la base de datos.");
                return BadRequest("Error al guardar la reseña.");
            }

            // --- CORRECCIÓN FINAL ---
            // Construimos el DTO de respuesta incluyendo el ID generado
            var resultDto = new ReviewDTO
            {
                ReviewId = review.ReviewId, // ESTO ES LA CLAVE PARA QUE FUNCIONE EL FRONTEND
                ReviewTitle = review.ReviewTitle,
                CustomerCountry = review.CustomerCountry,
                DateOfReview = review.DateOfReview,
                Username = review.User?.UserName ?? (string.IsNullOrWhiteSpace(input.Username) ? "Anonymous" : input.Username),

            };

            return Ok(resultDto);
        }
    }
}