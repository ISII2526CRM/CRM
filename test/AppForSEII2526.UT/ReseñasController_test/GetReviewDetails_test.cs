using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.ReseñasDTOs;
using AppForSEII2526.UT;
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
    public class GetReviewDetails_test : AppForSEII25264SqliteUT
    {
        public GetReviewDetails_test()
        {
            // Fixture común: modelos y dispositivos
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
        public async Task GetReviewDetails_NotFound_ReturnsNotFound()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ReviewsController>>();
            var controller = new ReviewsController(_context, mockLogger.Object);

            // Act
            var result = await controller.GetReviewDetails(id: 9999); // id que no existe

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetReviewDetails_Found_ReturnsExpectedDto()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ReviewsController>>();
            var controller = new ReviewsController(_context, mockLogger.Object);

            // Crear usuario
            var user = new ApplicationUser { UserName = "reviewer1" };
            _context.Add(user);

            // Obtener un dispositivo existente
            var device = _context.Device.First();

            // Crear reseña con ítem
            var review = new Review
            {
                User = user,
                CustomerCountry = "España",
                ReviewTitle = "Muy buena compra",
                DateOfReview = new DateTime(2024, 1, 2),
                ReviewItems = new List<ReviewItem>
                {
                    new ReviewItem
                    {
                        Device = device,
                        Rating = 5,
                        Comments = "Excelente rendimiento"
                    }
                }
            };

            _context.Add(review);
            _context.SaveChanges();

            var reviewId = review.ReviewId;

            // Act
            var actionResult = await controller.GetReviewDetails(reviewId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var dto = Assert.IsType<ReviewDTO>(okResult.Value);

            Assert.Equal(user.UserName, dto.Username);
            Assert.Equal("España", dto.CustomerCountry);
            Assert.Equal("Muy buena compra", dto.ReviewTitle);
            Assert.Equal(new DateTime(2024, 1, 2), dto.DateOfReview);

            Assert.NotNull(dto.ReviewItems);
            Assert.Single(dto.ReviewItems);

            var item = dto.ReviewItems.First();
            Assert.Equal(device.Name, item.DeviceName);
            Assert.Equal(device.Model.NameModel, item.DeviceModel);
            Assert.Equal(device.Year, item.DeviceYear);
            Assert.Equal(5, item.Rating);
            Assert.Equal("Excelente rendimiento", item.Comment);
        }


        [Fact]
        public async Task GetReviewDetails_ReviewHasNoItems_ReturnsOkWithEmptyList()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ReviewsController>>();
            var controller = new ReviewsController(_context, mockLogger.Object);

            var user = new ApplicationUser { UserName = "reviewer2" };
            _context.Add(user);

            var review = new Review
            {
                User = user,
                CustomerCountry = "México",
                ReviewTitle = "Sin items",
                ReviewItems = new List<ReviewItem>() // Lista VACÍA
            };
            _context.Add(review);
            _context.SaveChanges();

            // Act
            var actionResult = await controller.GetReviewDetails(review.ReviewId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var dto = Assert.IsType<ReviewDTO>(okResult.Value);

            // Verificamos que la lista existe, pero está vacía
            Assert.NotNull(dto.ReviewItems);
            Assert.Empty(dto.ReviewItems);
        }

        [Fact]
        public async Task GetReviewDetails_UserIsNull_ReturnsOkWithNullUsername()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ReviewsController>>();
            var controller = new ReviewsController(_context, mockLogger.Object);

            // Creamos una Review pero User es null (COMPORTAMIENTO ESPERADO)
            var review = new Review
            {
                User = null, // <-- Dato nulo
                ReviewTitle = "Reseña huérfana",
                CustomerCountry = "UK",
                DateOfReview = DateTime.Now
            };
            _context.Add(review);
            _context.SaveChanges();

            // Act
            var actionResult = await controller.GetReviewDetails(review.ReviewId);

            // Assert
            // 1. Verificamos que devuelve OK
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var dto = Assert.IsType<ReviewDTO>(okResult.Value);

            // 2. Verificamos que el Username es null (que es el comportamiento opcional correcto)
            Assert.Null(dto.Username);
            Assert.Equal("Reseña huérfana", dto.ReviewTitle); // Los otros datos sí están
        }

    }
}
