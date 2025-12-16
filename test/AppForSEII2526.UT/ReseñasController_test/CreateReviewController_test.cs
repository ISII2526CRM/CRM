using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.ReseñasDTOs;
using AppForSEII2526.API.Models; // Asegúrate de tener este using
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Necesario para validar manualmente
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AppForSEII2526.UT.ReseñasController_test
{
    public class CreateReviewController_test : AppForSEII25264SqliteUT
    {
        public CreateReviewController_test()
        {
            // Fixture: modelos y dispositivos
            var models = new List<Model>
            {
                new Model("ordenador"),
                new Model("teclado")
            };

            var devices = new List<Device>
            {
                new Device(models[0], "XPS 15", "Dell", "Plata", 1850.99, 120.00, 10, 3, 2023),
                new Device(models[1], "MX Keys S", "Logitech", "Grafito", 109.99, 15.00, 50, 20, 2023)
            };

            _context.AddRange(models);
            _context.AddRange(devices);
            _context.SaveChanges();
        }

        [Fact]
        public async Task Create_WithMissingDevice_ReturnsBadRequest()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ReviewsController>>();
            var controller = new ReviewsController(_context, mockLogger.Object);

            var user = new ApplicationUser { UserName = "bob" };
            _context.Add(user);
            _context.SaveChanges();

            var dto = new CreateReviewDTO
            {
                ReviewTitle = "Prueba sin device",
                CustomerCountry = "ES",
                Username = "bob",
                ReviewItems = new List<ReviewItemDTO>
                {
                    // Usamos el constructor (int, int, string) que definimos en el paso anterior
                    new ReviewItemDTO(9999, 5, "No existe")
                }
            };

            // Act
            var result = await controller.Create(dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Create_WithNonExistingUsername_ReturnsBadRequest()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ReviewsController>>();
            var controller = new ReviewsController(_context, mockLogger.Object);
            var device = _context.Device.First();

            var dto = new CreateReviewDTO
            {
                ReviewTitle = "Usuario no existe",
                CustomerCountry = "ES",
                Username = "noexiste",
                ReviewItems = new List<ReviewItemDTO>
                {
                    new ReviewItemDTO(device.Id, 3, "OK")
                }
            };

            // Act
            var result = await controller.Create(dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Create_WithInvalidRating_ReturnsBadRequest()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ReviewsController>>();
            var controller = new ReviewsController(_context, mockLogger.Object);

            var user = new ApplicationUser { UserName = "carol" };
            _context.Add(user);
            _context.SaveChanges();
            var device = _context.Device.First();

            var dto = new CreateReviewDTO
            {
                ReviewTitle = "Rating inválido",
                CustomerCountry = "ES",
                Username = "carol",
                ReviewItems = new List<ReviewItemDTO>
                {
                    // Rating 10 es inválido por el [Range(1,5)]
                    new ReviewItemDTO(device.Id, 10, "Mal rating")
                }
            };

            // --- TRUCO: Simular la validación del Modelo manualmente ---
            // Esto es necesario porque en Unit Tests el [ApiController] no actúa automáticamente
            var validationContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();
            // Validamos el DTO principal
            Validator.TryValidateObject(dto, validationContext, validationResults, true);

            // Validamos también los items de la lista (porque Range está dentro de ReviewItemDTO)
            foreach (var item in dto.ReviewItems)
            {
                var itemContext = new ValidationContext(item);
                Validator.TryValidateObject(item, itemContext, validationResults, true);
            }

            // Si hay errores de validación, los metemos al controlador
            if (validationResults.Any())
            {
                controller.ModelState.AddModelError("Rating", "Range error simulated");
            }
            // -----------------------------------------------------------

            // Act
            var result = await controller.Create(dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        // --- ESTE ES EL TEST NUEVO QUE TE FALTABA ---
        [Fact]
        public async Task Create_ValidData_ReturnsOkAndDto()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ReviewsController>>();
            var controller = new ReviewsController(_context, mockLogger.Object);

            var user = new ApplicationUser { UserName = "alice" };
            _context.Add(user);
            _context.SaveChanges();
            var device = _context.Device.First();

            var createDto = new CreateReviewDTO
            {
                ReviewTitle = "Todo correcto",
                CustomerCountry = "FR",
                Username = "alice",
                ReviewItems = new List<ReviewItemDTO>
                {
                    new ReviewItemDTO(device.Id, 5, "Reseña para que")
                }
            };

            // Act
            var result = await controller.Create(createDto);

            // Assert
            // Esperamos un OkObjectResult (o CreatedAtActionResult si usaste CreatedAtAction)
            // Si tu controlador devuelve Ok(resultDto), usa OkObjectResult.
            var okResult = Assert.IsType<OkObjectResult>(result);

            var returnedDto = Assert.IsType<ReviewDTO>(okResult.Value);

            // Verificamos datos clave
            Assert.Equal("Todo correcto", returnedDto.ReviewTitle);
            Assert.Equal("alice", returnedDto.Username);

            // Verificamos que se haya guardado en BBDD
            Assert.Equal(1, _context.Review.Count());
        }
    }
}