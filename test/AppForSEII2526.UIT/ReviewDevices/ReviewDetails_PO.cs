using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.ReviewDevices
{
    public class ReviewDetails_PO : PageObject
    {
        public ReviewDetails_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        public bool CheckReviewDetails(string title, string country, string username)
        {
            WaitForBeingVisible(By.Id("ReviewTitle"));

            bool result = true;
            result &= _driver.FindElement(By.Id("ReviewTitle")).Text.Contains(title);
            result &= _driver.FindElement(By.Id("CustomerCountry")).Text.Contains(country);

            var dateElementText = _driver.FindElement(By.Id("DateOfReview")).Text;
            result &= dateElementText.Contains(DateTime.Today.ToShortDateString());

            return result;
        }

        public bool CheckExactReviewItem(string expectedName, string expectedModel, string expectedYear, string expectedRating, string expectedComment)
        {
            WaitForBeingVisible(By.Id("ReviewItems"));

            try
            {
                var row = _driver.FindElement(By.XPath($"//table[@id='ReviewItems']/tbody/tr[td[1][normalize-space(text())='{expectedName}']]"));
                var columns = row.FindElements(By.TagName("td"));

                if (columns.Count < 5) return false;

                bool isNameExact = columns[0].Text.Trim() == expectedName;
                bool isModelExact = columns[1].Text.Trim() == expectedModel;
                bool isYearExact = columns[2].Text.Trim() == expectedYear;
                bool isRatingExact = columns[3].Text.Trim() == $"{expectedRating} / 5";
                bool isCommentExact = columns[4].Text.Trim() == expectedComment;

                return isNameExact && isModelExact && isYearExact && isRatingExact && isCommentExact;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public bool CheckListOfReviewItems(List<string[]> expectedReviewItems)
        {
            return CheckBodyTable(expectedReviewItems, By.Id("ReviewItems"));
        }
    }
}