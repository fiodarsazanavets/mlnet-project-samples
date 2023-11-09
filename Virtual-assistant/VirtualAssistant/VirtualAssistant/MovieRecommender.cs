using static MovieRecommendations.ConsoleApp.MovieRecommendations;

namespace VirtualAssistant;

internal static class MovieRecommender
{
    public static void CheckIfMovieIsGood()
    {
        Console.WriteLine("What is your user identifier?");
        var userId = float.Parse(Console.ReadLine());
        Console.WriteLine("What is the identifier of the movie you are interested in?");
        var movieId = float.Parse(Console.ReadLine());

        var input = new ModelInput
        {
            UserId = userId,
            MovieId = movieId
        };

        var prediction = Predict(input);

        Console.WriteLine(
            $"You will probably find this movie to be {GetHumanReadableRating(prediction.Score)}");
    }

    private static string GetHumanReadableRating(float rating)
    {
        switch (rating)
        {
            case var _ when rating < 1:
                return "horrible";
            case var _ when rating < 2:
                return "bad";
            case var _ when rating < 3:
                return "average";
            case var _ when rating < 4:
                return "good";
            default:
                return "awesome";
        }
    }
}
