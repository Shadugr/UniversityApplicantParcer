using HtmlAgilityPack;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium;

namespace UniversityApplicantParcer
{
    internal class ApplicantDictionary
    {
        public Dictionary<uint, Applicant> Applicants { get; set; }

        public ApplicantDictionary() => Applicants = new Dictionary<uint, Applicant>();
        public ApplicantDictionary(Dictionary<uint, Applicant> applicants) => Applicants = applicants;
        public ApplicantDictionary(params Applicant[] applicants)
        {
            Applicants = new Dictionary<uint, Applicant>();
            foreach (var applicant in applicants) { Applicants.Add(applicant.Id, applicant); }
        }
        public ApplicantDictionary(string url) => Applicants = GetApplicantsDictionary(GetNode(url));

        private List<HtmlNode>? GetNode(string url)
        {
            var browser = new EdgeDriver(Environment.CurrentDirectory);
            browser.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
            browser.Navigate().GoToUrl(url);

            var web = browser.FindElement(By.TagName("html"));
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(web.GetAttribute("innerHTML"));
            var node = htmlDoc.DocumentNode
                .Descendants()
                .Where(node1 => node1.HasClass("offer-request")).Skip(1).ToList();
            return node;
        }
        private Dictionary<uint, Applicant> GetApplicantsDictionary(List<HtmlNode>? node)
        {
            if(node is null) { return new Dictionary<uint, Applicant>(); }

            var htmlDoc = new HtmlDocument();
            Dictionary<uint, Applicant> result = new();
            foreach (var item in node)
            {
                htmlDoc.LoadHtml(item.InnerHtml);
                var argument = htmlDoc.DocumentNode.SelectNodes("//div/div");
                uint id = uint.Parse(argument[0].InnerText);
                result.Add(id, 
                    new Applicant(id, argument[1].InnerText, 
                    argument[2].InnerText, char.Parse(argument[3].InnerText), false, false,
                    float.Parse(argument[6].InnerText)));
                if(argument[4].OuterHtml.Contains("od-1") ||
                    argument[5].OuterHtml.Contains("od-2")) { result[id].IsChoose = true; }
                if (argument[5].OuterHtml.Contains("od-1")) { result[id].IsDocument = true; }
            }
            return result;
        }

        public void PrintAll(int count = int.MaxValue)
        {
            if(count > Applicants.Count) { count = Applicants.Count; }
            if(count < 1) { count = 1; }

            for (int i = 1; i <= count; i++)
            {
                Console.WriteLine($"Id - {Applicants[(uint)i].Id}\nName - {Applicants[(uint)i].Name}\n" +
                    $"Status - {Applicants[(uint)i].Status}\nPriority - {Applicants[(uint)i].Priority}\n" +
                    $"Choose University - {Applicants[(uint)i].IsChoose}\n" +
                    $"Give Document - {Applicants[(uint)i].IsDocument}\nRating - {Applicants[(uint)i].Rating}\n\n");
            }
        }
    }
}
