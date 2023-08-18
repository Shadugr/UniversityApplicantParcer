using OfficeOpenXml;
using UniversityApplicantParcer;

internal class Program
{
    private const string FILE_NAME = "user.txt";
    private const string DEFAULT_URL = "https://vstup.edbo.gov.ua/offer/1218942/";

    private static string FilePath { get; } = AppDomain.CurrentDomain.BaseDirectory + @"\" + FILE_NAME;
    private static string ExcelFilePath { get; } = AppDomain.CurrentDomain.BaseDirectory + @"\" + "Applicants.xlsx";

    private static void Main()
    {
        FileInfo userTxt = new(FilePath);
        string url = !File.Exists(FilePath) || userTxt.Length == 0 ? SaveUserFile() : LoadUserFile();
        ApplicantDictionary applicantDictionary = new(url);
        Console.Clear();
        ConsoleKeyInfo input;
        do
        {
            Console.WriteLine($"URL - {url}\n1. Filter and print Excel File.\n2. Print to console.\n3. Change URL.\n4. Exit");
            input = Console.ReadKey();
            switch (input.Key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    Console.Clear();
                    Console.WriteLine("Include contract? [y/n]");
                    input = Console.ReadKey();
                    bool contract = input.Key != ConsoleKey.Y;
                    Console.Clear();
                    Console.WriteLine("Input minimum rating:");
                    float rating;
                    try { rating = float.Parse(Console.ReadLine()); }
                    catch { rating = 0f; }
                    Console.Clear();
                    Console.WriteLine("Input status:");
                    string status = Console.ReadLine();
                    ApplicantDictionary applicantFilter = applicantDictionary.Filter(contract, rating, status);
                    using (var package = new ExcelPackage(new FileInfo(ExcelFilePath)))
                    {
                        var worksheet = package.Workbook.Worksheets.Add($"Applicants_{DateTime.Now.Day}.{DateTime.Now.Month}.{DateTime.Now.Year}" +
                            $"_{DateTime.Now.Hour}.{DateTime.Now.Minute}.{DateTime.Now.Second}");
                        worksheet.Cells[1, 1].Value = "#";
                        worksheet.Cells[1, 2].Value = "ПІБ";
                        worksheet.Cells[1, 3].Value = "Статус";
                        worksheet.Cells[1, 4].Value = "Пріорітет";
                        worksheet.Cells[1, 5].Value = "ПВМ";
                        worksheet.Cells[1, 6].Value = "Виконано вимоги";
                        worksheet.Cells[1, 7].Value = "Рейтинг";

                        int row = 2;
                        foreach (var applicant in applicantFilter.Applicants)
                        {
                            worksheet.Cells[row, 1].Value = applicant.Value.Id;
                            worksheet.Cells[row, 2].Value = applicant.Value.Name;
                            worksheet.Cells[row, 3].Value = applicant.Value.Status;
                            worksheet.Cells[row, 4].Value = applicant.Value.Priority;
                            worksheet.Cells[row, 5].Value = applicant.Value.IsChoose;
                            worksheet.Cells[row, 6].Value = applicant.Value.IsDocument;
                            worksheet.Cells[row, 7].Value = applicant.Value.Rating;
                            row++;
                        }
                        package.SaveAs(new FileInfo(ExcelFilePath));
                    }
                    Console.WriteLine("Press any key to return at menu.");
                    Console.ReadKey();
                    break;
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    Console.Clear();
                    Console.WriteLine("Input count (Default is all applicants):");
                    int count;
                    try { count = int.Parse(Console.ReadLine()); }
                    catch { count = 0; }
                    applicantDictionary.PrintAll(count);
                    Console.WriteLine("\nPress any key to return at menu.");
                    Console.ReadKey();
                    break;
                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    Console.Clear();
                    url = SaveUserFile();
                    Console.WriteLine("Press any key to return at menu.");
                    Console.ReadKey();
                    break;
                default:
                    break;
            }
            Console.Clear();
        } while (input.Key != ConsoleKey.D4 && input.Key != ConsoleKey.NumPad4);
    }

    private static string SaveUserFile()
    {
        Console.WriteLine("Input URL (If empty default master 122 Computer Science):");
        string url = Console.ReadLine();
        if (string.IsNullOrEmpty(url)) { url = DEFAULT_URL; }
        File.WriteAllText(FilePath, url );
        return url;
    }
    private static string LoadUserFile()
    {
        string url = File.ReadAllText(FilePath);
        Console.WriteLine($"File loaded.\nURL - {url}");
        return url;
    }
}