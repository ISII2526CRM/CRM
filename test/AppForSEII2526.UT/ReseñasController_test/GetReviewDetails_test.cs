using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.ReseñasDTOs;
using AppForSEII2526.API.Models;
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
            var mockLogger = new Mock<ILogger<ReviewsController>>();
            var controller = new ReviewsController(_context, mockLogger.Object);

            var result = await controller.GetReviewDetails(9999);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetReviewDetails_Found_ReturnsExpectedDto()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ReviewsController>>();
            var controller = new ReviewsController(_context, mockLogger.Object);

            var user = new ApplicationUser { UserName = "reviewer1" };
            _context.Add(user);
            var device = _context.Device.First();

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

            // --- PREPARAMOS EL OBJETO ESPERADO (EXPECTED) ---
            var expectedDTO = new ReviewDTO
            {
                ReviewId = review.ReviewId,
                Username = "reviewer1",
                CustomerCountry = "España",
                ReviewTitle = "Muy buena compra",
                DateOfReview = new DateTime(2024, 1, 2),
                ReviewItems = new List<ReviewItemDTO>
                {
                    new ReviewItemDTO
                    {
                        // Nota: El controller rellena estos datos usando Include
                        DeviceName = device.Name,
                        DeviceModel = device.Model.NameModel,
                        DeviceYear = device.Year,
                        Rating = 5,
                        Comment = "Excelente rendimiento"
                    }
                }
            };

            // Act
            var actionResult = await controller.GetReviewDetails(review.ReviewId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var actualDTO = Assert.IsType<ReviewDTO>(okResult.Value);

            // ¡MAGIA! Gracias al override de Equals, esto compara todo el árbol de objetos
            Assert.Equal(expectedDTO, actualDTO);
        }

        [Fact]
        public async Task GetReviewDetails_ReviewHasNoItems_ReturnsOkWithEmptyList()
        {
            var mockLogger = new Mock<ILogger<ReviewsController>>();
            var controller = new ReviewsController(_context, mockLogger.Object);
            var user = new ApplicationUser { UserName = "reviewer2" };
            _context.Add(user);

            var review = new Review
            {
                User = user,
                CustomerCountry = "México",
                ReviewTitle = "Sin items",
                ReviewItems = new List<ReviewItem>()
            };
            _context.Add(review);
            _context.SaveChanges();

            // Expected
            var expectedDTO = new ReviewDTO
            {
                ReviewId = review.ReviewId,
                Username = "reviewer2",
                CustomerCountry = "México",
                ReviewTitle = "Sin items",
                DateOfReview = review.DateOfReview, // Usamos la del objeto original porque DateTime.Now cambia
                ReviewItems = new List<ReviewItemDTO>() // Lista vacía
            };

            // Act
            var actionResult = await controller.GetReviewDetails(review.ReviewId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var actualDTO = Assert.IsType<ReviewDTO>(okResult.Value);

            Assert.Equal(expectedDTO, actualDTO);
        }
    }
}