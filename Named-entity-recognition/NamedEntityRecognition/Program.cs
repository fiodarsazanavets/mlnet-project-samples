using Microsoft.ML.Data;
using Microsoft.ML;
using NamedEntityRecognition;
using Microsoft.ML.TorchSharp;
using Microsoft.ML.Transforms;
using Microsoft.ML.Tokenizers;

try
{
    MLContext context = new()
    {
        FallbackToCpu = true,
        GpuDeviceId = 0
    };

    IDataView labels = context.Data.LoadFromEnumerable(LabelsHelper.GetLabels());

    IDataView dataView = context.Data.LoadFromEnumerable(
        TrainingDataProcessor.LoadDataFromFile());

    EstimatorChain<ITransformer> chain = new();

    EstimatorChain<KeyToValueMappingTransformer> estimator = chain.Append(context.Transforms.Conversion.MapValueToKey("Label", keyData: labels))
       .Append(context.MulticlassClassification.Trainers.NamedEntityRecognition(outputColumnName: "Predictions"))
       .Append(context.Transforms.Conversion.MapKeyToValue("Predictions"));

    using TransformerChain<KeyToValueMappingTransformer> transformer = estimator.Fit(dataView);

    DataViewSchema transformerSchema = transformer.GetOutputSchema(dataView.Schema);

    string sentence = "Alice and Bob visited Paris in France and met with Microsoft";
    TokenizerResult Encoded = TokenizationHelper.Tokenize(sentence);

    PredictionEngine<Input, Output> engine = context.Model.CreatePredictionEngine<Input, Output>(transformer);
    Output predictions = engine.Predict(new Input { Sentence = sentence });

    Console.WriteLine(sentence);
    Console.WriteLine(string.Join(", ", predictions.Predictions));
    Console.ReadLine();
}
catch (Exception ex)
{

    Console.WriteLine($"Error: {ex.Message}");
    Console.ReadLine();
}