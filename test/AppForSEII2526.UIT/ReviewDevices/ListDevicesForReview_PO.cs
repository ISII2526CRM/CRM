using System;
using System.Collections.Generic;
using System.Threading;
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

        public void FilterDevices(string brand, string year)
        {
            Thread.Sleep(1000);

            if (!string.IsNullOrEmpty(brand))
            {
                var brandInput = _driver.FindElement(By.Id("filterBrand"));
                brandInput.Clear();
                brandInput.SendKeys(brand);
                Thread.Sleep(500);
            }

            if (!string.IsNullOrEmpty(year))
            {
                var yearInput = _driver.FindElement(By.Id("filterYear"));
                yearInput.Clear();
                yearInput.SendKeys(year);
                Thread.Sleep(500);
            }

            _driver.FindElement(By.Id("searchDevices")).Click();
            Thread.Sleep(1500);
        }

        public void SelectDevices(List<string> devicesToSelect)
        {
            WaitForBeingVisibleIgnoringExeptionTypes(By.Id("TableOfDevices"));

            foreach (var deviceName in devicesToSelect)
            {
                var safeName = deviceName.Replace(" ", "_");
                var xpathAdd = $"//button[@id='btn_add_{safeName}']";
                var buttonsAdd = _driver.FindElements(By.XPath(xpathAdd));

                if (buttonsAdd.Count > 0)
                {
                    buttonsAdd[0].Click();
                    Thread.Sleep(1000);
                }
                else
                {
                    var xpathSelected = $"//tr[contains(., '{deviceName}')]//button[contains(text(), 'Selected')]";
                    if (_driver.FindElements(By.XPath(xpathSelected)).Count == 0)
                    {
                        throw new Exception($"Button 'btn_add_{safeName}' not found for device '{deviceName}'.");
                    }
                }
            }
        }

        public void RemoveDeviceFromCart(string deviceName)
        {
            var btnToggle = _driver.FindElement(By.Id("showReviewCart"));
            if (btnToggle.Text.Contains("Show"))
            {
                btnToggle.Click();
                Thread.Sleep(500);
            }

            var safeName = deviceName.Replace(" ", "_");
            var buttonId = $"btn_remove_{safeName}";

            try
            {
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
                var btnRemove = wait.Until(d =>
                {
                    var el = d.FindElement(By.Id(buttonId));
                    return (el != null && el.Displayed && el.Enabled) ? el : null;
                });

                btnRemove.Click();
                Thread.Sleep(2000);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to remove device '{buttonId}': {ex.Message}");
            }
        }

        public void ClickReviewDevices()
        {
            _driver.FindElement(By.Id("StartReview")).Click();
        }

        public bool CheckListOfDevices(List<string[]> expectedDevices)
        {
            Thread.Sleep(1000);

            try
            {
                var rows = _driver.FindElements(By.CssSelector("#TableOfDevices tbody tr"));

                if (rows.Count != expectedDevices.Count) return false;

                for (int i = 0; i < expectedDevices.Count; i++)
                {
                    string rowText = rows[i].Text;
                    foreach (var expectedVal in expectedDevices[i])
                    {
                        if (!string.IsNullOrEmpty(expectedVal) && !rowText.Contains(expectedVal))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool CheckReviewButtonDisabled()
        {
            var btn = _driver.FindElement(By.Id("StartReview"));
            return !btn.Enabled;
        }

        public void ClearFilters()
        {
            Thread.Sleep(1000);
            var locator = By.XPath("//button[contains(text(), 'Clear')]");

            try
            {
                _driver.FindElement(locator).Click();
            }
            catch (StaleElementReferenceException)
            {
                Thread.Sleep(500);
                _driver.FindElement(locator).Click();
            }
            catch (NoSuchElementException) { }

            Thread.Sleep(1500);
        }
    }
}