using UniversityApplicantParcer;

internal class Program
{
    private static void Main(string[] args)
    {
        var url = @"https://vstup.edbo.gov.ua/offer/1203097/";
        ApplicantDictionary a = new(url);
        a.PrintAll(5);
    }
}