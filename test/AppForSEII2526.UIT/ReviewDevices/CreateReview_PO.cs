using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit.Abstractions; // Necesario para ITestOutputHelper

namespace AppForSEII2526.UIT.ReviewDevices
{
    
    public class CreateReview_PO : PageObject
    {
    public CreateReview_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

    public void FillInReviewInfo(string title, string country)
    {
        Thread.Sleep(1000); // Espera de seguridad al cargar la página

        if (!string.IsNullOrEmpty(title))
        {
            var txtTitle = _driver.FindElement(By.Id("ReviewTitle"));
            txtTitle.Clear();
            txtTitle.SendKeys(title);
        }

        if (!string.IsNullOrEmpty(country))
        {
            var txtCountry = _driver.FindElement(By.Id("CustomerCountry"));
            txtCountry.Clear();
            txtCountry.SendKeys(country);
        }
    }

        public void FillInRatingAndComment(string rating, string comment)
        {
            // --- 1. CAMBIAR EL RATING ---
            if (!string.IsNullOrEmpty(rating))
            {
                // Buscamos el input numérico dentro de la tabla (para no confundirlo con otros)
                // Usamos XPath para ir a lo seguro: primer input numérico en el cuerpo de la tabla
                var txtRating = _driver.FindElement(By.XPath("//tbody//input[@type='number']"));

                // TRUCO PRO: Los inputs numéricos a veces fallan con .Clear()
                // Lo mejor es: Seleccionar todo (Ctrl+A) -> Borrar -> Escribir
                txtRating.SendKeys(Keys.Control + "a");
                txtRating.SendKeys(Keys.Backspace);

                // Escribimos el número
                txtRating.SendKeys(rating);

                // ¡LA CLAVE! Pulsamos TAB para salir del campo.
                // Esto obliga a Blazor a disparar el evento "OnChange" y guardar el valor en la variable.
                txtRating.SendKeys(Keys.Tab);

                // Esperamos medio segundo a que Blazor procese el cambio
                Thread.Sleep(500);
            }

            // --- 2. CAMBIAR EL COMENTARIO ---
            if (!string.IsNullOrEmpty(comment))
            {
                // Buscamos el área de texto (textarea) o el input de texto en la tabla
                // Probamos primero con textarea, si no lo encuentra, busca input texto
                IWebElement txtComment;
                try
                {
                    txtComment = _driver.FindElement(By.XPath("//tbody//textarea"));
                }
                catch
                {
                    // Si tu componente usa un input normal en vez de textarea
                    txtComment = _driver.FindElement(By.XPath("//tbody//input[@type='text']"));
                }

                txtComment.Clear();
                txtComment.SendKeys(comment);

                // También pulsamos Tab aquí por seguridad
                txtComment.SendKeys(Keys.Tab);
                Thread.Sleep(500);
            }
        }
        public void PressSaveReview()
    {
        // PASO 1: Clic en "Submit Review" (Esto abre el Modal)
        var btnSubmit = _driver.FindElement(By.XPath("//button[@type='submit']"));
        btnSubmit.Click();

        // PASO 2: Esperar a que aparezca el Modal de confirmación
        Thread.Sleep(1000);

        // PASO 3: Confirmar en el Modal
        // Buscamos el botón que confirma la acción dentro del diálogo.
        // Normalmente en tus diálogos es el botón primario o dice "Yes"/"Save"
        try
        {
            // Intento 1: Buscar por clase de botón primario dentro del modal
            var btnConfirm = _driver.FindElement(By.XPath("//button[contains(text(), 'Submit') or contains(text(), 'Yes')]"));
                btnConfirm.Click();
        }
        catch
        {
            // Intento 2: Buscar por texto "Save" o "Yes" si el anterior falla
            var btnConfirm = _driver.FindElement(By.XPath("//button[contains(text(), 'Submit') or contains(text(), 'Yes')]"));
            btnConfirm.Click();
        }

        // PASO 4: Espera final para la navegación a Detalles
        // Ahora sí que navega de verdad
        Thread.Sleep(3000);
    }

    public void PressCancel()
    {
        var btnCancel = _driver.FindElement(By.XPath("//button[contains(text(), 'Cancel')]"));
        btnCancel.Click();
        Thread.Sleep(1000);
    }

    public bool CheckValidationError(string errorMessage)
    {
        Thread.Sleep(1000); // Esperar a que el validador muestre el mensaje
        return _driver.PageSource.Contains(errorMessage);
    }
}
}