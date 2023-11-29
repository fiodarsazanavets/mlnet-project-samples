using static MovieRecommendations.ConsoleApp.MovieRecommendations;

namespace VirtualAssistant;

internal static class MovieRecommender
{
    public static void CheckIfMovieIsGood()
    {
        Console.WriteLine("What is your user identifier?");
        float userId = float.Parse(Console.ReadLine() ?? string.Empty);
        Console.WriteLine("What is the identifier of the movie you are interested in?");
        float movieId = float.Parse(Console.ReadLine() ?? string.Empty);

        ModelInput input = new()
        {
            UserId = userId,
            MovieId = movieId
        };

        ModelOutput prediction = Predict(input);

        Console.WriteLine(
            $"You will probably find this movie to be {GetHumanReadableRating(prediction.Score)}");
    }

    private static string GetHumanReadableRating(float rating)
    {
        return rating switch
        {
            <= 1 => "horrible",
            <= 2 => "bad",
            <= 3 => "average",
            <= 4 => "good",
            _ => "awesome",
        };
    }
}
