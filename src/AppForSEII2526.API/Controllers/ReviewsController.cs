using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AppForSEII2526.API.DTOs.ReseñasDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
                .Select(r => new ReviewDTO(
                    // Constructor requiere 'username' (no null). Usamos string.Empty si User es null.
                    r.User != null ? r.User.UserName : string.Empty,
                    r.CustomerCountry,
                    r.ReviewTitle,
                    r.DateOfReview,
                    // Convertimos a List<ReviewItemDTO> usando el constructor requerido
                    r.ReviewItems.Select(item => new ReviewItemDTO(
                        item.Device.Name,
                        item.Device.Model.NameModel,
                        item.Device.Year,
                        item.Rating,
                        item.Comments
                    )).ToList()
                ))
                .FirstOrDefaultAsync(); // Usamos FirstOrDefaultAsync porque solo buscamos UNO

            if (reviewDetails == null)
            {
                _logger.LogWarning($"No se encontró ninguna reseña con el ID {id}.");
                return NotFound($"No se encontró ninguna reseña con el ID {id}.");
            }

            return Ok(reviewDetails);
        }

        [HttpPost]
        [Route("Create")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
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
                if (string.IsNullOrWhiteSpace(it.Comment))
                    return BadRequest("El comentario es obligatorio para cada dispositivo.");
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
                    
                    _logger.LogInformation("Usuario proporcionado en la reseña no existe: {Username}. La reseña se creará sin usuario asociado.", input.Username);
                }
            }

            // Crear entidad Review
            var review = new AppForSEII2526.API.Models.Review
            {
                ReviewTitle = input.ReviewTitle,
                CustomerCountry = input.CustomerCountry,
                DateOfReview = DateTime.UtcNow,
                // OverallRating opcional: se puede calcular como media de los items
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

            // Devolver Created con la localización al GET de detalles
            return CreatedAtAction(nameof(GetReviewDetails), new { id = review.ReviewId }, new { reviewId = review.ReviewId });
        }

    }
}