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
            Thread.Sleep(1500);

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
                txtRating.SendKeys(Keys.Tab);
                Thread.Sleep(500);
            }

            if (!string.IsNullOrEmpty(comment))
            {
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
            var btnSubmit = _driver.FindElement(By.XPath("//button[contains(text(), 'Submit Review')]"));
            btnSubmit.Click();
            Thread.Sleep(1000);

            try
            {
                _driver.FindElement(By.CssSelector(".modal-content .btn-primary")).Click();
            }
            catch
            {
                _driver.FindElement(By.XPath("//button[contains(text(), 'Save') or contains(text(), 'Yes')]")).Click();
            }

            Thread.Sleep(3000);
        }

        public void PressCancel()
        {
            Thread.Sleep(1000);
            _driver.FindElement(By.XPath("//button[contains(text(), 'Cancel')]")).Click();
            Thread.Sleep(1000);
        }

        public bool CheckValidationError(string expectedError)
        {
            Thread.Sleep(1000);

            try
            {
                var errorElements = _driver.FindElements(By.CssSelector(".validation-message, .validation-summary-errors li, .text-danger, ul li"));

                foreach (var element in errorElements)
                {
                    string actualText = element.Text.Trim();
                    if (!string.IsNullOrEmpty(actualText) && actualText.Contains(expectedError, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                
            }

            return _driver.PageSource.Contains(expectedError);
        }

        public void PressSubmit_ExpectingError()
        {
            var btnSubmit = _driver.FindElement(By.XPath("//button[contains(text(), 'Submit Review')]"));
            btnSubmit.Click();
            Thread.Sleep(1000);
        }

        public void FillInAllRatingsAndComments(string rating, string comment)
        {
            
            if (!string.IsNullOrEmpty(rating))
            {
                var ratingInputs = _driver.FindElements(By.CssSelector("input[type='number']"));
                foreach (var input in ratingInputs)
                {
                    input.SendKeys(Keys.Control + "a");
                    input.SendKeys(Keys.Backspace);
                    input.SendKeys(rating);
                    Thread.Sleep(200);
                }
            }

            var commentInputs = _driver.FindElements(By.TagName("textarea"));
            if (commentInputs.Count == 0)
            {
                commentInputs = _driver.FindElements(By.CssSelector("input[type='text']"));
            }

            foreach (var input in commentInputs)
            {
                input.Clear();
                if (!string.IsNullOrEmpty(comment))
                {
                    input.SendKeys(comment);
                }
                Thread.Sleep(200);
            }
        }
    }
}