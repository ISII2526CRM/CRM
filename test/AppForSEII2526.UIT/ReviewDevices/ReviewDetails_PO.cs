using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.ReviewDevices
{
    public class ReviewDetails_PO : PageObject
    {
        public ReviewDetails_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        // Método adaptado para verificar los datos de cabecera de la reseña
        public bool CheckReviewDetails(string title, string country, string username)
        {
            // Esperamos a que sea visible el título (señal de que la página cargó)
            WaitForBeingVisible(By.Id("ReviewTitle"));

            bool result = true;

            // Verificamos los textos simples
            // Usamos &= para acumular el resultado (si uno falla, todo es false)
            result = result && _driver.FindElement(By.Id("ReviewTitle")).Text.Contains(title);
            result = result && _driver.FindElement(By.Id("CustomerCountry")).Text.Contains(country);

            // Si en tu vista se muestra el usuario, descomenta esto:
            // result = result && _driver.FindElement(By.Id("Username")).Text.Contains(username);

            // 3. Verificamos la fecha 
            var dateElementText = _driver.FindElement(By.Id("DateOfReview")).Text;

            // Si el texto es solo fecha (ej: "12/12/2023")
            result = result && dateElementText.Contains(DateTime.Today.ToShortDateString());

          

            return result;
        }

        
        public bool CheckListOfReviewItems(List<string[]> expectedReviewItems)
        {
            return CheckBodyTable(expectedReviewItems, By.Id("ReviewItemsTable"));
        }
    }
}