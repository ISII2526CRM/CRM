using AppForMovies.UIT.Shared;
using AppForSEII2526.UIT.Shared;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.ReviewDevices
{
    public class CUReviewDevices_UIT : UC_UIT
    {
        private const string URI = "https://localhost:7083/";
        private ListDevicesForReview_PO _listDevicesPO;

        private const string Device1_Name = "XPS 15";
        private const string Device1_Brand = "Dell";
        private const string Device1_Year = "2023";
        private const string Device1_Model = "Standard";

        public CUReviewDevices_UIT(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            Initial_step_opening_the_web_page();
            _listDevicesPO = new ListDevicesForReview_PO(_driver, _output);
        }

        private void Precondition_perform_login()
        {
            Perform_login("alice@test.com", "Password.123");
        }

        private void InitialStepsForReviewDevices()
        {
            Precondition_perform_login();
            _driver.Navigate().GoToUrl(URI + "reviews/listdevices");
        }

        // TC-03 y TC-04: Filtros (Flujo Alternativo 0)
        [Theory]
        [InlineData("Dell", "", Device1_Name)]
        [InlineData("", "2023", Device1_Name)]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC3_AF0_FilteringByBrandAndYear(string brand, string year, string expectedDevice)
        {
            var expectedList = new List<string[]> {
                new string[] { expectedDevice, brand, year }
            };

            InitialStepsForReviewDevices();
            _listDevicesPO.FilterDevices(brand, year);

            Assert.True(_listDevicesPO.CheckListOfDevices(expectedList));
        }

        // TC-06: Carrito Vacío (Flujo Alternativo 2)
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC3_AF2_ReviewButtonDisabled_WhenCartEmpty()
        {
            InitialStepsForReviewDevices();
            Assert.True(_listDevicesPO.CheckReviewButtonDisabled(), "StartReview button should be disabled when cart is empty.");
        }

        // TC-05: Eliminar del carrito (Flujo Alternativo 1)
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC3_AF1_RemoveDeviceFromCart()
        {
            InitialStepsForReviewDevices();
            _listDevicesPO.FilterDevices(Device1_Brand, Device1_Year);
            _listDevicesPO.SelectDevices(new List<string> { Device1_Name });

            _listDevicesPO.RemoveDeviceFromCart(Device1_Name);

            Assert.True(_listDevicesPO.CheckReviewButtonDisabled(), "StartReview button should be disabled after removing the only device.");
        }

        // TC-07 y TC-08: Errores datos obligatorios (Flujo Alternativo 3)
        [Theory]
        [InlineData("", "Spain", "El título es obligatorio")]
        [InlineData("Titulo", "", "El país es obligatorio")]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC3_AF3_MandatoryDataErrors(string title, string country, string expectedError)
        {
            var createReviewPO = new CreateReview_PO(_driver, _output);

            InitialStepsForReviewDevices();
            _listDevicesPO.FilterDevices(Device1_Brand, Device1_Year);
            _listDevicesPO.SelectDevices(new List<string> { Device1_Name });
            _listDevicesPO.ClickReviewDevices();

            createReviewPO.FillInReviewInfo(title, country);
            createReviewPO.FillInRatingAndComment("5", "Valid comment");
            createReviewPO.PressSubmit_ExpectingError();

            Assert.True(createReviewPO.CheckValidationError(expectedError), $"Expected validation error not found: '{expectedError}'");
        }

        // TC-10: Error formato (Flujo Alternativo 5) - Puntuación > 5
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC3_AF5_InvalidRatingError()
        {
            var createReviewPO = new CreateReview_PO(_driver, _output);

            InitialStepsForReviewDevices();
            _listDevicesPO.FilterDevices(Device1_Brand, Device1_Year);
            _listDevicesPO.SelectDevices(new List<string> { Device1_Name });
            _listDevicesPO.ClickReviewDevices();

            createReviewPO.FillInReviewInfo("Titulo Test", "Spain");
            createReviewPO.FillInRatingAndComment("10", "Test comment");
            createReviewPO.PressSubmit_ExpectingError();

            Assert.Contains("createreview", _driver.Url.ToLower());
        }

        // TC-09: Cancelar (Flujo Alternativo 4)
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC3_AF4_CancelOperation()
        {
            var createReviewPO = new CreateReview_PO(_driver, _output);

            InitialStepsForReviewDevices();
            _listDevicesPO.SelectDevices(new List<string> { Device1_Name });
            _listDevicesPO.ClickReviewDevices();

            createReviewPO.PressCancel();

            Assert.Contains("listdevices", _driver.Url.ToLower());
        }

        // TC-01: Flujo Básico (Creación Correcta)
        [Theory]
        [InlineData("Review Perfecta", "Spain", "5", "Excelente dispositivo")]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC3_BasicFlow_CreateReviewSuccessful(string title, string country, string rating, string comment)
        {
            var createReviewPO = new CreateReview_PO(_driver, _output);
            var detailsPO = new ReviewDetails_PO(_driver, _output);

            InitialStepsForReviewDevices();
            _listDevicesPO.FilterDevices(Device1_Brand, Device1_Year);
            _listDevicesPO.SelectDevices(new List<string> { Device1_Name });
            _listDevicesPO.ClickReviewDevices();

            createReviewPO.FillInReviewInfo(title, country);
            createReviewPO.FillInRatingAndComment(rating, comment);
            createReviewPO.PressSaveReview();

            try
            {
                var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(_driver, TimeSpan.FromSeconds(5));
                wait.Until(d => d.Url.ToLower().Contains("detailreview"));
            }
            catch
            {
                Assert.Contains("detailreview", _driver.Url.ToLower());
            }

            Assert.True(detailsPO.CheckReviewDetails(title, country, string.Empty), "Header details mismatch.");

            string expectedFullComment = $"Reseña para {Device1_Name}: {comment}";

            bool isExactObjectPresent = detailsPO.CheckExactReviewItem(
                expectedName: Device1_Name,
                expectedModel: Device1_Model,
                expectedYear: Device1_Year,
                expectedRating: rating,
                expectedComment: expectedFullComment 
            );

            Assert.True(isExactObjectPresent, "Exact review item mapping not found in details table.");
        }


        // TC-02: Flujo Básico (Múltiples Dispositivos)
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC3_BasicFlow_MultipleReviewSuccessful()
        {
            var createReviewPO = new CreateReview_PO(_driver, _output);
            var detailsPO = new ReviewDetails_PO(_driver, _output);

            InitialStepsForReviewDevices();

            _listDevicesPO.SelectDevices(new List<string> { Device1_Name, "MX Keys S" });
            _listDevicesPO.ClickReviewDevices();

            string title = "Pack Oficina";
            string country = "France";
            string rating = "4";
            string comment = ""; 

            createReviewPO.FillInReviewInfo(title, country);
            createReviewPO.FillInAllRatingsAndComments(rating, comment);
            createReviewPO.PressSaveReview();

            try
            {
                var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(_driver, TimeSpan.FromSeconds(5));
                wait.Until(d => d.Url.ToLower().Contains("detailreview"));
            }
            catch
            {
                Assert.Contains("detailreview", _driver.Url.ToLower());
            }

            Assert.True(detailsPO.CheckReviewDetails(title, country, string.Empty), "Header details mismatch.");

            bool isDevice1Present = detailsPO.CheckExactReviewItem(
                expectedName: Device1_Name,
                expectedModel: Device1_Model,
                expectedYear: Device1_Year,
                expectedRating: rating,
                expectedComment: $"Reseña para {Device1_Name}:"
            );
            Assert.True(isDevice1Present, "El XPS 15 no se guardó correctamente en la reseña múltiple.");

            bool isDevice2Present = detailsPO.CheckExactReviewItem(
                expectedName: "MX Keys S",
                expectedModel: "Standard", 
                expectedYear: "2024",
                expectedRating: rating,
                expectedComment: "Reseña para MX Keys S:"
            );
            Assert.True(isDevice2Present, "El MX Keys S no se guardó correctamente en la reseña múltiple.");
        }

        // Examen Sprint 3 extraordinaria
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void ExamenSprint3()
        {
            var createReviewPO = new CreateReview_PO(_driver, _output);
            var detailsPO = new ReviewDetails_PO(_driver, _output);

            InitialStepsForReviewDevices();
            _listDevicesPO.FilterDevices("Logitech", " ");
            _listDevicesPO.SelectDevices(new List<string> { "MX Keys S", "Otro" });

            _listDevicesPO.FilterDevices(" ", Device1_Year);
            _listDevicesPO.SelectDevices(new List<string> { Device1_Name });

            _listDevicesPO.RemoveDeviceFromCart("MX Keys S");
            _listDevicesPO.RemoveDeviceFromCart("Otro");
            _listDevicesPO.ClickReviewDevices();

            string title = "Examen Sprint3";
            string country = "France";
            string rating = "5";
            string comment = "¡Va genial!";

            createReviewPO.FillInReviewInfo(title, country);
            createReviewPO.FillInAllRatingsAndComments(rating, comment);
            createReviewPO.PressSaveReview();

            try
            {
                var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(_driver, TimeSpan.FromSeconds(5));
                wait.Until(d => d.Url.ToLower().Contains("detailreview"));
            }
            catch
            {
                Assert.Contains("detailreview", _driver.Url.ToLower());
            }

            Assert.True(detailsPO.CheckReviewDetails(title, country, string.Empty), "Header details mismatch.");

            string expectedFullComment = $"Reseña para {Device1_Name}: {comment}";

            bool isExactObjectPresent = detailsPO.CheckExactReviewItem(
                expectedName: Device1_Name,
                expectedModel: Device1_Model,
                expectedYear: Device1_Year,
                expectedRating: rating,
                expectedComment: expectedFullComment
            );

            Assert.True(isExactObjectPresent, "Exact review item mapping not found in details table.");

        }
    }
}