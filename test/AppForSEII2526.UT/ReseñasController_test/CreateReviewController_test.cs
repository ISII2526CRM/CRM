using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.ReseñasDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AppForSEII2526.UT.ReseñasController_test
{
    public class CreateReviewController_test : AppForSEII25264SqliteUT
    {
        public CreateReviewController_test()
        {
            // Fixture: modelos y dispositivos (igual que en otros tests)
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
        public async Task Create_WithExistingUser_ReturnsCreatedAndPersists()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ReviewsController>>();
            var controller = new ReviewsController(_context, mockLogger.Object);

            var user = new ApplicationUser { UserName = "alice" };
            _context.Add(user);
            _context.SaveChanges();

            var device = _context.Device.First();

            var dto = new CreateReviewDTO
            {
                ReviewTitle = "Prueba creación",
                CustomerCountry = "ES",
                Username = "alice",
                ReviewItems = new List<ReviewItemDTO>
                {
                    new ReviewItemDTO(device.Id, 4, "Bien")
                }
            };

            // Act
            var result = await controller.Create(dto);

            // Assert: Created
            var created = Assert.IsType<CreatedAtActionResult>(result);
            // Comprobar que se guardó en BD
            var saved = _context.Review
                .Include(r => r.User)
                .Include(r => r.ReviewItems)
                    .ThenInclude(ri => ri.Device)
                .FirstOrDefault(r => r.ReviewTitle == dto.ReviewTitle);

            Assert.NotNull(saved);
            Assert.NotNull(saved.User);
            Assert.Equal(user.UserName, saved.User.UserName);
            Assert.Single(saved.ReviewItems);
            Assert.Equal(4, saved.ReviewItems.First().Rating);
            Assert.Equal("Bien", saved.ReviewItems.First().Comments);
            Assert.Equal(device.Id, saved.ReviewItems.First().DeviceId);
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
                    new ReviewItemDTO(device.Id, 10, "Mal rating")
                }
            };

            // Act
            var result = await controller.Create(dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
