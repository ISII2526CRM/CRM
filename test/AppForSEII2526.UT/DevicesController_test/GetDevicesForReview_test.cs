using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.ReseñasDTOs;
using AppForSEII2526.UT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                new Device (models[0], "XPS 15", "Dell", "Plata", 1850.99, 120.00, 10, 3, 2023),
                new Device (models[1], "Galaxy S24 Ultra", "Samsung", "Negro Titanio", 1400.00, 90.50, 25, 10, 2024),
                new Device (models[2], "MX Keys S", "Logitech", "Grafito", 109.99, 15.00, 50, 20, 2023),
                new Device (models[3], "MX Master 3S", "Logitech", "Negro", 99.50, 10.00, 60, 30, 2022)
            };

            _context.AddRange(models);
            // --- ¡AQUÍ ESTÁ LA CORRECCIÓN! ---
            _context.AddRange(devices); // Debe ser AddRange, no Add
            _context.SaveChanges();
        }


        [Fact]
        [Trait("Database", "Sqlite")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetDeviceForReviewFiltro_SinFiltros_DevuelveTodosLosDevices()
        {
            // Arrange
            // Creamos un Logger "falso" (Mock)
            var mockLogger = new Mock<ILogger<DevicesController>>();

            // Creamos el controlador, pasándole nuestro contexto de BBDD y el logger
            var controller = new DevicesController(_context, mockLogger.Object);

            // Act
            // Llamamos al método con los filtros en null
            var result = await controller.GetDeviceForReviewFiltro(Brand: null, Year: null);

            // Assert
            // 1. Verificamos que el resultado es un 'OkObjectResult' (HTTP 200)
            var okResult = Assert.IsType<OkObjectResult>(result);

            // 2. Verificamos que el valor que contiene es una Lista de DTOs
            var dtos = Assert.IsType<List<DevicesReseñaDTO>>(okResult.Value);

            // 3. Verificamos que la lista tiene 4 elementos (todos los que guardamos)
            Assert.Equal(4, dtos.Count);
        }
    }
}
