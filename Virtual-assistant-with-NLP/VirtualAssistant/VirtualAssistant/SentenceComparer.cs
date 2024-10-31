using static SentenceSimilarity.SentenceSimilarity;

namespace VirtualAssistant;

public static class SentenceComparer
{
    public static void CompareSentences()
    {
        Console.WriteLine("Please enter the first sentence.");
        string sentence1 = Console.ReadLine() ?? string.Empty;

        Console.WriteLine("Please enter the second sentence.");
        string sentence2 = Console.ReadLine() ?? string.Empty;

        ModelInput input = new()
        {
            Sentence1 = sentence1,
            Sentence2 = sentence2,
        };

        ModelOutput? output = Predict(input);

        Console.WriteLine($"The predicted sentence similarity is {output?.Score ?? 0}.");
    }
}
