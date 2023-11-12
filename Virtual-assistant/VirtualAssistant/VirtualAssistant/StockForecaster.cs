using static AmazonStockPredictions.AmazonStockPredictions;

namespace VirtualAssistant;

internal static class StockForecaster
{
    public static void ForecastStockPrices()
    {
        Console.WriteLine("How many working days do you want to predict stock prices for?");
        var horizon = int.Parse(Console.ReadLine());

        var input = new ModelInput();

        var predictions = Predict(input, horizon);

        Console.WriteLine("Forecasted stock prices:");

        foreach (var stockPrice in predictions.AdjClose)
        {
            Console.WriteLine(stockPrice);
        }
    }
}