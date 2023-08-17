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

        private static List<HtmlNode>? GetNode(string url)
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
        private static Dictionary<uint, Applicant> GetApplicantsDictionary(List<HtmlNode>? node)
        {
            if(node is null) { return new Dictionary<uint, Applicant>(); }

            var htmlDoc = new HtmlDocument();
            Dictionary<uint, Applicant> result = new();
            foreach (var item in node)
            {
                htmlDoc.LoadHtml(item.InnerHtml);
                var argument = htmlDoc.DocumentNode.SelectNodes("//div/div");
                uint id = uint.Parse(argument[0].InnerText);
                string name = argument[1].InnerText;
                float rating = float.Parse(argument[6].InnerText);
                if (rating <= 80f && ApplicantResults.Results.Exists(a => a.Name == name))
                {
                    rating += ApplicantResults.Results.First(a => a.Name == name).Result * 0.6f;
                }
                result.Add(id, 
                    new Applicant(id, name, argument[2].InnerText, char.Parse(argument[3].InnerText), false, false, rating));
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

            Dictionary<uint, Applicant> filter = Applicants.Take(count).OrderByDescending(u => u.Value.Rating).ToDictionary(n => n.Key, n=> n.Value);

            foreach (var applicant in filter)
            {
                Console.WriteLine($"Id - {applicant.Value.Id}\nName - {applicant.Value.Name}\n" +
                    $"Status - {applicant.Value.Status}\nPriority - {applicant.Value.Priority}\n" +
                    $"Choose University - {applicant.Value.IsChoose}\n" +
                    $"Give Document - {applicant.Value.IsDocument}\nRating - {applicant.Value.Rating}\n\n");
            }
        }
        public ApplicantDictionary Filter(bool isContractOut = false, float minRating = 0f, string status = "")
        {
            var filterDictionary = Applicants.OrderByDescending(u => u.Value.Rating)
                .OrderByDescending(u => u.Value.Rating)
                .ToDictionary(n => n.Key, n => n.Value);
            if(isContractOut) 
            {
                filterDictionary = filterDictionary.Where(n => n.Value.Priority != 'К')
                    .ToDictionary(n => n.Key, n => n.Value); 
            }
            if (minRating > 0f)
            {
                filterDictionary = filterDictionary.Where(n => n.Value.Rating >= minRating)
                    .ToDictionary(n => n.Key, n => n.Value);
            }
            if (!string.IsNullOrEmpty(status))
            {
                filterDictionary = filterDictionary.Where(n => n.Value.Status == status)
                    .ToDictionary(n => n.Key, n => n.Value);
            }
            return new ApplicantDictionary(filterDictionary);
        }
    }
}
