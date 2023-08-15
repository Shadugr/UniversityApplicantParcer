using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

internal class Program
{
    private static void Main(string[] args)
    {
        var url = @"https://vstup.edbo.gov.ua/offer/1218942/";
        var browser = new EdgeDriver(Environment.CurrentDirectory);
        browser.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
        browser.Navigate().GoToUrl(url);

        var web = browser.FindElement(By.TagName("html"));
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(web.GetAttribute("innerHTML"));
        var node = htmlDoc.DocumentNode
            .Descendants()
            .Where(node1 => node1.HasClass("offer-request")).Skip(1).ToList();
        Console.WriteLine(node[0].InnerText);
    }
}