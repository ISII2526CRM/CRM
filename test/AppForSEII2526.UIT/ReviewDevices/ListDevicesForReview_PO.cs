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
            Thread.Sleep(500); // Dar tiempo a Blazor para procesar el binding
        }

        if (!string.IsNullOrEmpty(year))
        {
            var yearInput = _driver.FindElement(By.Id("filterYear"));
            yearInput.Clear();
            yearInput.SendKeys(year);
            Thread.Sleep(500);
        }

        // Click en buscar y espera IMPORTANTE para que la tabla se refresque
        _driver.FindElement(By.Id("searchDevices")).Click();
        Thread.Sleep(1500);
    }

    public void SelectDevices(List<string> devicesToSelect)
    {
        // Aseguramos que la tabla es visible antes de empezar
        WaitForBeingVisibleIgnoringExeptionTypes(By.Id("TableOfDevices"));

        foreach (var deviceName in devicesToSelect)
        {
            // LÓGICA INTELIGENTE:
            // Intentamos encontrar el botón de añadir usando XPath (maneja espacios mejor que ID)
            // XPath busca: un botón cuyo ID sea 'btn_add_NombreDispositivo'
            var xpathAdd = $"//button[@id='btn_add_{deviceName}']";
            var buttonsAdd = _driver.FindElements(By.XPath(xpathAdd));

            if (buttonsAdd.Count > 0)
            {
                // CASO 1: El botón de añadir existe -> Lo pulsamos
                buttonsAdd[0].Click();
                Thread.Sleep(1000); // Esperamos a que se procese la acción
            }
            else
            {
                // CASO 2: No está el botón de añadir. ¿Quizás ya está seleccionado?
                // Buscamos si en la fila de ese dispositivo hay un botón que ponga "Selected"
                var xpathSelected = $"//tr[contains(., '{deviceName}')]//button[contains(text(), 'Selected')]";
                if (_driver.FindElements(By.XPath(xpathSelected)).Count > 0)
                {
                    // Ya estaba seleccionado. No hacemos nada y seguimos.
                    // Esto evita que el test falle si repites la prueba sin limpiar la BD.
                }
                else
                {
                    // CASO 3: Ni botón Add ni botón Selected. El dispositivo no está en la lista.
                    throw new Exception($"ERROR: No encuentro el dispositivo '{deviceName}' en la tabla con los filtros actuales.");
                }
            }
        }
    }

    public void RemoveDeviceFromCart(string deviceName)
    {
        // PASO 1: Garantizar que el carrito está visible
        var btnToggle = _driver.FindElement(By.Id("showReviewCart"));

        // Si el botón dice "Show", significa que el carrito está oculto. Click para abrir.
        if (btnToggle.Text.Contains("Show"))
        {
            btnToggle.Click();
            Thread.Sleep(1000); // Esperamos la animación de apertura
        }

        // PASO 2: Buscar el botón de borrar y pulsarlo
        // Usamos XPath porque el ID tiene espacios ("XPS 15") y By.Id falla a veces.
        try
        {
            var xpathRemove = $"//button[@id='btn_remove_{deviceName}']";
            var btnRemove = _driver.FindElement(By.XPath(xpathRemove));

            btnRemove.Click();

            // PASO 3: Espera crítica para que Blazor actualice el estado
            // El botón "Review" debe pasar a Disabled.
            Thread.Sleep(2000);
        }
        catch (NoSuchElementException)
        {
            throw new Exception($"El carrito está abierto pero no encuentro el botón 'Remove' para '{deviceName}'.");
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
}
}