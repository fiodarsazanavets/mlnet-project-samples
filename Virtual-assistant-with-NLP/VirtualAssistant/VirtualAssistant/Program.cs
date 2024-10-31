using VirtualAssistant;

Console.WriteLine("Welcome to the Virtual Assistant!");
bool continueRunning = true;

while (continueRunning)
{
    string? command = ChatbotInputProcessor.ProcessChatInput();

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
        case "errors":
            ErrorCategorizer.CategorizeErrors();
            break;
        case "anomalies":
            AnomalyDetector.DetectAnomalies();
            break;
        case "sentence similarity":
            SentenceComparer.CompareSentences();
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