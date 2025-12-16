using AppForMovies.UIT.Shared; // Ojo: Asegúrate de que este namespace es correcto en tu proyecto (quizás sea AppForSEII2526.UIT.Shared)
using AppForSEII2526.UIT.ReviewDevices;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.ReviewDevices
{
    public class CUReviewDevices_UIT : UC_UIT
    {
        private const string URI = "https://localhost:7083/";
        // Variable privada para el PageObject inicial
        private ListDevicesForReview_PO _listDevicesPO;

        // CONSTANTES PARA DATOS DE PRUEBA (Deben coincidir con tu BD/Seed)
        private const string Device1_Name = "XPS 15";
        private const string Device1_Brand = "Dell";
        private const string Device1_Year = "2023";

        // Constructor
        public CUReviewDevices_UIT(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            // 1. Abrimos el navegador (Método de UC_UIT)
            Initial_step_opening_the_web_page();

            // 2. Inicializamos el PO de la lista para empezar a trabajar
            _listDevicesPO = new ListDevicesForReview_PO(_driver, _output);
        }

        // --- MÉTODOS AUXILIARES (Precondiciones) ---

        private void Precondition_perform_login()
        {
            // Ajusta usuario y contraseña válidos
            Perform_login("alice@test.com", "Password.123");
        }

        private void InitialStepsForReviewDevices()
        {
            Precondition_perform_login();

            // Navegamos a la URL de la lista de dispositivos para reseñar
            // Opción A: Si tienes un ID en el menú, úsalo:
            // _listDevicesPO.WaitForBeingVisibleIgnoringExeptionTypes(By.Id("MenuReviews"));
            // _driver.FindElement(By.Id("MenuReviews")).Click();

            // Opción B: Navegación directa (más seguro para test):
            _driver.Navigate().GoToUrl(URI + "reviews/listdevices");
        }

        // --- CASOS DE PRUEBA (TEST CASES) ---

        // TC-03 y TC-04: Filtros (Flujo Alternativo 0)
        [Theory]
        [InlineData("Dell", "", Device1_Name)]       // Filtro solo Marca
        [InlineData("", "2023", Device1_Name)]       // Filtro solo Año
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC3_AF0_FilteringByBrandAndYear(string brand, string year, string expectedDevice)
        {
            // Arrange
            var expectedList = new List<string[]> {
                new string[] { expectedDevice, brand, year } // Ajusta columnas a tu tabla real
            };

            // Act
            InitialStepsForReviewDevices();
            _listDevicesPO.FilterDevices(brand, year);

            // Assert
            Assert.True(_listDevicesPO.CheckListOfDevices(expectedList));
        }

        // TC-06: Carrito Vacío (Flujo Alternativo 2)
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC3_AF2_ReviewButtonDisabled_WhenCartEmpty()
        {
            // Act
            InitialStepsForReviewDevices();
            // No seleccionamos nada

            // Assert
            Assert.True(_listDevicesPO.CheckReviewButtonDisabled(), "El botón debería estar deshabilitado si no selecciono nada");
        }

        // TC-05: Eliminar del carrito (Flujo Alternativo 1)
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC3_AF1_RemoveDeviceFromCart()
        {
            // Act
            InitialStepsForReviewDevices();
            _listDevicesPO.SelectDevices(new List<string> { Device1_Name });

            // Lo eliminamos (asumiendo que al seleccionar aparece opción de quitar o modificamos el carrito)
            _listDevicesPO.RemoveDeviceFromCart(Device1_Name);

            // Assert
            // Verificamos que ya no está marcado o en la lista de seleccionados
            // (Ajusta esto según cómo funcione tu UI: si desaparece de una lista lateral, etc.)
            // Ejemplo: Comprobamos que el botón vuelve a estar deshabilitado
            Assert.True(_listDevicesPO.CheckReviewButtonDisabled());
        }

        // TC-07 y TC-08: Errores datos obligatorios (Flujo Alternativo 3)
        [Theory]
        [InlineData("", "Spain", "The ReviewTitle field is required")]
        [InlineData("Titulo", "", "The CustomerCountry field is required")]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC3_AF3_MandatoryDataErrors(string title, string country, string expectedError)
        {
            // Arrange
            var createReviewPO = new CreateReview_PO(_driver, _output);

            // Act
            InitialStepsForReviewDevices();
            _listDevicesPO.SelectDevices(new List<string> { Device1_Name });
            _listDevicesPO.ClickReviewDevices();

            createReviewPO.FillInReviewInfo(title, country);
            createReviewPO.PressSaveReview();

            // Assert
            Assert.True(createReviewPO.CheckValidationError(expectedError), $"Esperaba error: {expectedError}");
        }

        // TC-10: Error formato (Flujo Alternativo 5) - Puntuación > 5
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC3_AF5_InvalidRatingError()
        {
            // Arrange
            var createReviewPO = new CreateReview_PO(_driver, _output);
            string expectedError = "between 1 and 5"; // Ajusta al texto exacto de tu error

            // Act
            InitialStepsForReviewDevices();
            _listDevicesPO.SelectDevices(new List<string> { Device1_Name });
            _listDevicesPO.ClickReviewDevices();

            createReviewPO.FillInReviewInfo("Titulo Test", "Spain");
            createReviewPO.FillInRatingAndComment("10", "Comentario"); // 10 es inválido
            createReviewPO.PressSaveReview();

            // Assert
            Assert.True(createReviewPO.CheckValidationError(expectedError));
        }

        // TC-09: Cancelar (Flujo Alternativo 4)
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC3_AF4_CancelOperation()
        {
            // Arrange
            var createReviewPO = new CreateReview_PO(_driver, _output);

            // Act
            InitialStepsForReviewDevices();
            _listDevicesPO.SelectDevices(new List<string> { Device1_Name });
            _listDevicesPO.ClickReviewDevices();

            createReviewPO.PressCancel();

            // Assert
            // Comprobamos vuelta a la lista por la URL
            Assert.Contains("listdevices", _driver.Url.ToLower());
        }

        // TC-01: Flujo Básico (Creación Correcta)
        [Theory]
        [InlineData("Review Perfecta", "Spain", "5", "Excelente dispositivo")]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC3_BasicFlow_CreateReviewSuccessful(string title, string country, string rating, string comment)
        {
            // Arrange
            var createReviewPO = new CreateReview_PO(_driver, _output);
            var detailsPO = new ReviewDetails_PO(_driver, _output);

            // Act
            InitialStepsForReviewDevices();

            // 1. Selección
            _listDevicesPO.SelectDevices(new List<string> { Device1_Name });
            _listDevicesPO.ClickReviewDevices();

            // 2. Creación
            createReviewPO.FillInReviewInfo(title, country);
            createReviewPO.FillInRatingAndComment(rating, comment);
            createReviewPO.PressSaveReview();

            // 3. Verificación (Asserts)

            // Verificamos cabecera
            Assert.True(detailsPO.CheckReviewDetails(title, country, "alice@test.com"));

            // Verificamos items (Ajustando la lógica de 'Reseña para...')
            string expectedComment = comment.StartsWith("Reseña para") ? comment : $"Reseña para {Device1_Name}: {comment}";

            var expectedItems = new List<string[]> {
                new string[] { Device1_Name, Device1_Brand, Device1_Year, rating, expectedComment }
            };

            Assert.True(detailsPO.CheckListOfReviewItems(expectedItems), "La tabla de items creados no coincide");
        }
    }
}