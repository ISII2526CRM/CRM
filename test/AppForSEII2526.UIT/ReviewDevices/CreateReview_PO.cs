using System;
using System.Threading;
using OpenQA.Selenium;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.ReviewDevices
{
    public class CreateReview_PO : PageObject
    {
        public CreateReview_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        public void FillInReviewInfo(string title, string country)
        {
            Thread.Sleep(1500); // Espera vital para que cargue la página nueva

            if (!string.IsNullOrEmpty(title))
            {
                var el = _driver.FindElement(By.Id("ReviewTitle"));
                el.Clear(); el.SendKeys(title);
            }
            if (!string.IsNullOrEmpty(country))
            {
                var el = _driver.FindElement(By.Id("CustomerCountry"));
                el.Clear(); el.SendKeys(country);
            }
        }

        public void FillInRatingAndComment(string rating, string comment)
        {
           
            if (!string.IsNullOrEmpty(rating))
            {
                var txtRating = _driver.FindElement(By.CssSelector("input[type='number']"));
                txtRating.SendKeys(Keys.Control + "a");
                txtRating.SendKeys(Keys.Backspace);
                txtRating.SendKeys(rating);
                txtRating.SendKeys(Keys.Tab); // Obliga a Blazor a guardar
                Thread.Sleep(500);
            }

            
            if (!string.IsNullOrEmpty(comment))
            {
                // Intenta textarea, si falla busca input text
                IWebElement txtComment;
                try { txtComment = _driver.FindElement(By.TagName("textarea")); }
                catch { txtComment = _driver.FindElement(By.CssSelector("input[type='text']")); }

                txtComment.Clear();
                txtComment.SendKeys(comment);
                txtComment.SendKeys(Keys.Tab);
                Thread.Sleep(500);
            }
        }

        public void PressSaveReview()
        {
            // 1. Click en Submit (Buscando por texto para no darle a Remove)
            var btnSubmit = _driver.FindElement(By.XPath("//button[contains(text(), 'Submit Review')]"));
            btnSubmit.Click();
            Thread.Sleep(1000);

            // 2. Confirmar Modal
            try
            {
                // Intenta botón primario del modal
                _driver.FindElement(By.CssSelector(".modal-content .btn-primary")).Click();
            }
            catch
            {
                // Intenta por texto
                _driver.FindElement(By.XPath("//button[contains(text(), 'Save') or contains(text(), 'Yes')]")).Click();
            }

            Thread.Sleep(3000); // Espera navegación final
        }

        public void PressCancel()
        {
            System.Threading.Thread.Sleep(1500);

            _driver.FindElement(By.XPath("//button[contains(text(), 'Cancel')]")).Click();
            Thread.Sleep(1000);
        }

        public bool CheckValidationError(string expectedError)
        {
            // 1. Esperamos un poco a que aparezca el mensaje rojo
            System.Threading.Thread.Sleep(1000);

            try
            {
               
                // Buscamos elementos que Blazor usa para mostrar errores

                var errorElements = _driver.FindElements(By.CssSelector(".validation-message, .validation-summary-errors li, .text-danger, ul li"));

                foreach (var element in errorElements)
                {
                    // Limpiamos espacios y saltos de línea para comparar mejor
                    string actualText = element.Text.Trim();

                    

                    if (!string.IsNullOrEmpty(actualText) && actualText.Contains(expectedError, StringComparison.OrdinalIgnoreCase))
                    {
                        return true; 
                    }
                }
            }
            catch (Exception)
            {
                // Si falla la búsqueda específica, no pasa nada, devolvemos false.
            }

            
            return _driver.PageSource.Contains(expectedError);
        }


        public void PressSubmit_ExpectingError()
        {
            // Solo hacemos clic en Submit. 
            // NO buscamos el modal porque si hay error, el modal no saldrá.
            var btnSubmit = _driver.FindElement(By.XPath("//button[contains(text(), 'Submit Review')]"));
            btnSubmit.Click();

            // Esperamos un poco a que aparezcan los mensajes rojos
            System.Threading.Thread.Sleep(1000);
        }
    }
}