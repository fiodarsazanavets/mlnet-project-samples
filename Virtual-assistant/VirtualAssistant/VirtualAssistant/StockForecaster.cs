using static AmazonStockPredictions.AmazonStockPredictions;

namespace VirtualAssistant;

internal static class StockForecaster
{
    public static void ForecastStockPrices()
    {
        Console.WriteLine("How many working days do you want to predict stock prices for?");
        int horizon = int.Parse(Console.ReadLine() ?? string.Empty);

        ModelInput input = new();

        ModelOutput predictions = Predict(input, horizon);

        Console.WriteLine("Forecasted stock prices:");

        foreach (float stockPrice in predictions.AdjClose)
        {
            Console.WriteLine(stockPrice);
        }
    }
}