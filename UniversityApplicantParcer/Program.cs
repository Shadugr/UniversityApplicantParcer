using OfficeOpenXml;
using UniversityApplicantParcer;

internal class Program
{
    private const string FILE_NAME = "user.txt";

    private static string FolderPath { get; } = AppDomain.CurrentDomain.BaseDirectory + @"\userdata";
    private static string UserFilePath { get; } = FolderPath + @"\" + FILE_NAME;
    private static string ExcelFilePath { get; } = FolderPath + @"\" + "Applicants.xlsx";

    private static void Main()
    {
        Directory.CreateDirectory(FolderPath);
        FileInfo userTxt = new(UserFilePath);
        string url = !File.Exists(UserFilePath) || userTxt.Length == 0 ? SaveUserFile() : LoadUserFile();
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
                    Console.Clear();
                    Console.WriteLine("Excel file created\nPress any key to return at menu.");
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
                    break;
                default:
                    break;
            }
            Console.Clear();
        } while (input.Key != ConsoleKey.D4 && input.Key != ConsoleKey.NumPad4);
    }

    private static string SaveUserFile()
    {
        string url = "";
        while (string.IsNullOrEmpty(url))
        {
            Console.WriteLine("Input URL (If empty default master 122 Computer Science):");
            url = Console.ReadLine();
            File.WriteAllText(UserFilePath, url);
            Console.Clear();
        }
        return url;
    }
    private static string LoadUserFile()
    {
        string url = File.ReadAllText(UserFilePath);
        Console.WriteLine($"File loaded.\nURL - {url}");
        Console.Clear();
        return url;
    }
}