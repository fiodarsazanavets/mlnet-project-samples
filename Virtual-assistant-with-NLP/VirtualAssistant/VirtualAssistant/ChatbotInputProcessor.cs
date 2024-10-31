using static TextClassification.TextClassification;

namespace VirtualAssistant;

public static class ChatbotInputProcessor
{
    public static string ProcessChatInput()
    {
        Console.WriteLine("What can I help you with today?");
        string question = Console.ReadLine() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(question))
        {
            return string.Empty;
        }

        ModelInput input = new()
        {
            Text = question,
        };

        ModelOutput output = Predict(input);

        return output.PredictedLabel;
    }
}
