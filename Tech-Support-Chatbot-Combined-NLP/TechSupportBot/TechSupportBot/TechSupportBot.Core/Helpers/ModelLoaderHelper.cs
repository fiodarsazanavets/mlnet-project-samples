using Microsoft.ML;

namespace TechSupportBot.Core.Helpers;

public static class ModelLoaderHelper
{
    public static ITransformer LoadModel(MLContext mlContext, string modelPath)
    {
        if (!File.Exists(modelPath))
        {
            throw new FileNotFoundException(
                $"The ML.NET model file was not found: {modelPath}",
                modelPath);
        }
        return mlContext.Model.Load(modelPath, out _);
    }
}
