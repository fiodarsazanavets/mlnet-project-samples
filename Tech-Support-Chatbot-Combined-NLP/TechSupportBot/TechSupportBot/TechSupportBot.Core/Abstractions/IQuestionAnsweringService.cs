using TechSupportBot.Core.Data;

namespace TechSupportBot.Core.Abstractions;

public interface IQuestionAnsweringService
{
    QuestionAnsweringPrediction Predict(QuestionAnsweringData input);
}
