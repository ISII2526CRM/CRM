using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading; // Necesario para Thread.Sleep
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.ReviewDevices
{
    public class ListDevicesForReview_PO : PageObject
    {
    public ListDevicesForReview_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

    // --- MÉTODOS DE ACCIÓN ---

    public void FilterDevices(string brand, string year)
    {
        // Espera inicial de carga
        Thread.Sleep(1000);

        if (!string.IsNullOrEmpty(brand))
        {
            var brandInput = _driver.FindElement(By.Id("filterBrand"));
            brandInput.Clear();
            brandInput.SendKeys(brand);
            Thread.Sleep(500); // Dar tiempo a Blazor 
        }

        if (!string.IsNullOrEmpty(year))
        {
            var yearInput = _driver.FindElement(By.Id("filterYear"));
            yearInput.Clear();
            yearInput.SendKeys(year);
            Thread.Sleep(500);
        }

        // Click en buscar y espera
        _driver.FindElement(By.Id("searchDevices")).Click();
        Thread.Sleep(1500);
    }

        public void SelectDevices(List<string> devicesToSelect)
        {
            // Esperamos que la tabla sea visible
            WaitForBeingVisibleIgnoringExeptionTypes(By.Id("TableOfDevices"));

            foreach (var deviceName in devicesToSelect)
            {
                // El Razor ahora genera IDs con guiones bajos (XPS_15).
                // Tenemos que transformar el nombre igual que lo hace el Razor.
                var safeName = deviceName.Replace(" ", "_");

                // Buscamos el ID exacto: 'btn_add_XPS_15'
                var xpathAdd = $"//button[@id='btn_add_{safeName}']";
                var buttonsAdd = _driver.FindElements(By.XPath(xpathAdd));

                if (buttonsAdd.Count > 0)
                {
                    // Encontrado el botón de añadir -> Click
                    buttonsAdd[0].Click();

                    // Espera para que Blazor procese el clic y cambie el botón a verde
                    System.Threading.Thread.Sleep(1000);
                }
                else
                {
                    // No encuentro el botón Add.
                    // Aquí buscamos por texto dentro de la fila, así que el ID no importa tanto.
                    var xpathSelected = $"//tr[contains(., '{deviceName}')]//button[contains(text(), 'Selected')]";

                    if (_driver.FindElements(By.XPath(xpathSelected)).Count > 0)
                    {
                        // Ya estaba seleccionado. Todo ok.
                    }
                    else
                    {

                        // No encuentro ni el botón Add ni el botón Selected
                        throw new Exception($"ERROR: Veo la tabla, pero no encuentro el botón con ID 'btn_add_{safeName}' para el dispositivo '{deviceName}'.");
                    }
                }
            }
        }


        public void RemoveDeviceFromCart(string deviceName)
        {
           
            var safeName = deviceName.Replace(" ", "_");
            var buttonId = $"btn_remove_{safeName}";

            try
            {
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));

                // Espera manual hasta que el elemento sea visible y esté habilitado
                var btnRemove = wait.Until(d =>
                {
                    var el = d.FindElement(By.Id(buttonId));
                    return (el != null && el.Displayed && el.Enabled) ? el : null;
                });

                btnRemove.Click();

                // Esperar a que desaparezca
                System.Threading.Thread.Sleep(2000);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al borrar '{buttonId}': {ex.Message}");
            }
        }

        public void ClickReviewDevices()
    {
        _driver.FindElement(By.Id("StartReview")).Click();
    }

    // --- MÉTODOS DE COMPROBACIÓN ---

    public bool CheckListOfDevices(List<string[]> expectedDevices)
    {
        Thread.Sleep(500); // Pequeña espera por seguridad

        // Intentamos localizar la tabla. Si no aparece en 5 segs, fallará (gestionado por UC_UIT)
        try
        {
            var table = _driver.FindElement(By.Id("TableOfDevices"));
        }
        catch { return false; }

        var rows = _driver.FindElements(By.CssSelector("#TableOfDevices tbody tr"));
        return rows.Count == expectedDevices.Count;
    }

    public bool CheckReviewButtonDisabled()
    {
        var btn = _driver.FindElement(By.Id("StartReview"));
        // Devolvemos true si NO está habilitado
        return !btn.Enabled;
    }


        public void ClearFilters()
        {
            // Espera de seguridad
            System.Threading.Thread.Sleep(1000);

            // Definimos cómo buscar el botón
            var locator = By.XPath("//button[contains(text(), 'Clear')]");

            try
            {
                // Buscar y Clickar
                var btnClear = _driver.FindElement(locator);
                btnClear.Click();
            }
            catch (StaleElementReferenceException)
            {
                // EXCEPCIÓN CAZADA

                System.Threading.Thread.Sleep(500); // Pequeña pausa para asegurar
                var btnClear = _driver.FindElement(locator);
                btnClear.Click();
            }
            catch (NoSuchElementException)
            {
                
            }

            // Esperamos a que la tabla se recargue 
            System.Threading.Thread.Sleep(1500);
        }
    }

}