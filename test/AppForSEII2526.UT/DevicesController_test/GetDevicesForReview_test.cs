using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.ReseñasDTOs;
using AppForSEII2526.API.Models; // <--- FALTABA ESTE (Para Device y Model)
using AppForSEII2526.UT;
using Microsoft.AspNetCore.Mvc; // <--- FALTABA ESTE (Para OkObjectResult)
using Microsoft.Extensions.Logging; // <--- FALTABA ESTE (Para ILogger)
using Moq; // <--- FALTABA ESTE (Para Mock)
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AppForSEII2526.UT.DevicesController_test
{
    public class GetDevicesForReview_test : AppForSEII25264SqliteUT
    {
        public GetDevicesForReview_test()
        {
            var models = new List<Model>()
            {
                new Model ("ordenador"),
                new Model ("teléfono"),
                new Model ("Teclado"),
                new Model ("Ratón")
            };

            var devices = new List<Device>()
            {
                // Orden: Model, Name, Brand, Color, Price, Weight, Stock, Warranty, Year
                new Device (models[0], "XPS 15", "Dell", "Plata", 1850.99, 120.00, 10, 3, 2023),
                new Device (models[1], "Galaxy S24 Ultra", "Samsung", "Negro Titanio", 1400.00, 90.50, 25, 10, 2024),
                new Device (models[2], "MX Keys S", "Logitech", "Grafito", 109.99, 15.00, 50, 20, 2023),
                new Device (models[3], "MX Master 3S", "Logitech", "Negro", 99.50, 10.00, 60, 30, 2022)
            };

            _context.AddRange(models);
            _context.AddRange(devices);
            _context.SaveChanges();
        }

        [Fact]
        [Trait("Database", "Sqlite")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetDeviceForReviewFiltro_SinFiltros_DevuelveTodosLosDevices()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DevicesController>>();
            var controller = new DevicesController(_context, mockLogger.Object);

            // Act
            var result = await controller.GetDeviceForReviewFiltro(Brand: null, Year: null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsType<List<DevicesReseñaDTO>>(okResult.Value);

            // 3. Verificamos valores. 
            // IMPORTANTE: He actualizado el constructor para incluir el NOMBRE ("XPS 15", etc.)
            // que añadimos en la interfaz hace un momento.
            var expectedDTOs = new List<DevicesReseñaDTO>()
            {
                // Constructor: (Id, Name, Brand, Color, Year, ModelName)
                new DevicesReseñaDTO(1, "XPS 15",           "Dell",    "Plata",         2023, "ordenador"),
                new DevicesReseñaDTO(2, "Galaxy S24 Ultra", "Samsung", "Negro Titanio", 2024, "teléfono"),
                new DevicesReseñaDTO(3, "MX Keys S",        "Logitech","Grafito",       2023, "Teclado"),
                new DevicesReseñaDTO(4, "MX Master 3S",     "Logitech","Negro",         2022, "Ratón")
            };

            var orderedExpected = expectedDTOs.OrderBy(d => d.Id).ToList();
            var orderedActual = dtos.OrderBy(d => d.Id).ToList();

            // Nota: Esto funciona si DevicesReseñaDTO es un 'record' o tiene Equals sobrescrito.
            Assert.Equal(orderedExpected, orderedActual);
        }

        [Fact]
        [Trait("Database", "Sqlite")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetDeviceForReviewFiltro_FiltroSoloPorBrand_DevuelveDevicesCorrectos()
        {
            // Arrange 
            var mockLogger = new Mock<ILogger<DevicesController>>();
            var controller = new DevicesController(_context, mockLogger.Object);

            // Definimos los DTOs que esperamos (solo los Logitech)
            var expectedDTOs = new List<DevicesReseñaDTO>()
            {
                new DevicesReseñaDTO(3, "MX Keys S",    "Logitech", "Grafito", 2023, "Teclado"),
                new DevicesReseñaDTO(4, "MX Master 3S", "Logitech", "Negro",   2022, "Ratón")
            };
            var orderedExpectedDTOs = expectedDTOs.OrderBy(d => d.Id).ToList();

            // Act 
            var result = await controller.GetDeviceForReviewFiltro(Brand: "Logitech", Year: null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualDTOs = Assert.IsType<List<DevicesReseñaDTO>>(okResult.Value);

            Assert.Equal(2, actualDTOs.Count);

            var orderedActualDTOs = actualDTOs.OrderBy(d => d.Id).ToList();
            Assert.Equal(orderedExpectedDTOs, orderedActualDTOs);
        }

        [Fact]
        [Trait("Database", "Sqlite")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetDeviceForReviewFiltro_FiltroSoloPorYear_DevuelveDevicesCorrectos()
        {
            // Arrange 
            var mockLogger = new Mock<ILogger<DevicesController>>();
            var controller = new DevicesController(_context, mockLogger.Object);

            // Definimos los DTOs que esperamos (solo los de 2023)
            var expectedDTOs = new List<DevicesReseñaDTO>()
            {
                new DevicesReseñaDTO(1, "XPS 15",    "Dell",     "Plata",   2023, "ordenador"),
                new DevicesReseñaDTO(3, "MX Keys S", "Logitech", "Grafito", 2023, "Teclado")
            };
            var orderedExpectedDTOs = expectedDTOs.OrderBy(d => d.Id).ToList();

            // Act 
            var result = await controller.GetDeviceForReviewFiltro(Brand: null, Year: 2023);

            // Assert 
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualDTOs = Assert.IsType<List<DevicesReseñaDTO>>(okResult.Value);

            Assert.Equal(2, actualDTOs.Count);

            var orderedActualDTOs = actualDTOs.OrderBy(d => d.Id).ToList();
            Assert.Equal(orderedExpectedDTOs, orderedActualDTOs);
        }

        [Fact]
        [Trait("Database", "Sqlite")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetDeviceForReviewFiltro_FiltroPorBrandYYear_DevuelveDeviceCorrecto()
        {
            // Arrange 
            var mockLogger = new Mock<ILogger<DevicesController>>();
            var controller = new DevicesController(_context, mockLogger.Object);

            // Definimos el DTO que esperamos (solo el Logitech de 2023)
            var expectedDTOs = new List<DevicesReseñaDTO>()
            {
                new DevicesReseñaDTO(3, "MX Keys S", "Logitech", "Grafito", 2023, "Teclado")
            };
            var orderedExpectedDTOs = expectedDTOs.OrderBy(d => d.Id).ToList();

            // Act 
            var result = await controller.GetDeviceForReviewFiltro(Brand: "Logitech", Year: 2023);

            // Assert 
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualDTOs = Assert.IsType<List<DevicesReseñaDTO>>(okResult.Value);

            Assert.Equal(1, actualDTOs.Count);

            var orderedActualDTOs = actualDTOs.OrderBy(d => d.Id).ToList();
            Assert.Equal(orderedExpectedDTOs, orderedActualDTOs);
        }
    }
}