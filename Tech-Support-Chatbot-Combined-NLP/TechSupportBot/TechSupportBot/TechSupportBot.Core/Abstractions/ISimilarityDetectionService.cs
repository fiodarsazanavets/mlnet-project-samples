using TechSupportBot.Core.Data;

namespace TechSupportBot.Core.Abstractions;

public interface ISimilarityDetectionService
{
    SimilarityPrediction Predict(SimilarityData input);
}
