using static TechJobsSalaries.ConsoleApp.TechJobsSalaries;

namespace VirtualAssistant;

internal static class TechSalaries
{
    public static void PredictSalary()
    {
        Console.WriteLine("Please specify your age");
        var age = float.Parse(Console.ReadLine());
        Console.WriteLine("Please specify your experience in months");
        var experienceInMonths = float.Parse(Console.ReadLine());
        Console.WriteLine("Please specify the job title");
        var jobTitle = Console.ReadLine();
        Console.WriteLine("Please specify the department name");
        var department = Console.ReadLine();

        var input = new ModelInput
        {
            Age = age,
            TenureInOrgInMonths = experienceInMonths,
            JobTitle = jobTitle,
            Department = department
        };

        var prediction = Predict(input);

        Console.WriteLine($"The expected salary is {prediction.Score}.");
    }
}
