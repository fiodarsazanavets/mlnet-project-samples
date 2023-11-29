using VirtualAssistant;

Console.WriteLine("Welcome to the Virtual Assistant!");
bool continueRunning = true;

while (continueRunning)
{
    Console.WriteLine("Type 'tech support' to obtain a tech support problem category.");
    Console.WriteLine("Type 'salary' to estimate a salary for a tech job.");
    Console.WriteLine("Type 'movies' to find out if a movie is likely to be good.");
    Console.WriteLine("Type 'stocks' to forecast Amazon stock prices.");
    Console.WriteLine("Type 'stop' to exit the app.");
    string? command = Console.ReadLine();

    switch (command)
    {
        case "tech support":
            TechSupport.ClassifyProblem();
            break;
        case "salary":
            TechSalaries.PredictSalary();
            break;
        case "movies":
            MovieRecommender.CheckIfMovieIsGood();
            break;
        case "stocks":
            StockForecaster.ForecastStockPrices();
            break;
        case "stop":
            continueRunning = false;
            break;
        default:
            Console.WriteLine("Unknown command. Please try again.");
            break;
    }

    Console.WriteLine(string.Empty);
    Console.WriteLine("---");
    Console.WriteLine(string.Empty);

}