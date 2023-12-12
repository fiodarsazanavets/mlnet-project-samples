using static TechJobsSalaries.ConsoleApp.TechJobsSalaries;

namespace VirtualAssistant;

internal static class TechSalaries
{
    public static void PredictSalary()
    {
        Console.WriteLine("Please specify your age");
        float age = float.Parse(Console.ReadLine() ?? string.Empty);
        Console.WriteLine("Please specify your experience in months");
        float experienceInMonths = float.Parse(Console.ReadLine() ?? string.Empty);
        Console.WriteLine("Please specify the job title");
        string? jobTitle = Console.ReadLine();
        Console.WriteLine("Please specify the department name");
        string? department = Console.ReadLine();

        ModelInput input = new()
        {
            Age = age,
            TenureInOrgInMonths = experienceInMonths,
            JobTitle = jobTitle,
            Department = department
        };

        ModelOutput prediction = Predict(input);

        Console.WriteLine($"The expected salary is {prediction.Score}.");
    }
}
