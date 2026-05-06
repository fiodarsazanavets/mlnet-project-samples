using TechSupportBot.Core.Data;

namespace TechSupportBot.Core.Abstractions;

public interface INamedEntityRecognitionService
{
    NamedEntityPrediction Predict(NamedEntityData input);
}
