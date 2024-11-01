using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;

namespace NamedEntityRecognition;

public static class TrainingDataProcessor
{
    public static IEnumerable<Input> LoadDataFromFile()
    {
        Console.WriteLine("Please provide the path to the input file.");
        string inputFilePath = Console.ReadLine() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(inputFilePath))
        {
            throw new ArgumentException("Cannot");
        }

        if (!File.Exists(inputFilePath))
        {
            throw new ArgumentException("A file does not exist at the path specified");
        }

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            TrimOptions = TrimOptions.Trim,
        };

        List<Input> inputs = [];

        using (var reader = new StreamReader(inputFilePath))
        using (var csv = new CsvReader(reader, config))
        {
            csv.Read();

            while (csv.Read())
            {
                var sentence = csv.GetField(0);
                var labelString = csv.GetField(1)?.Replace("\"", string.Empty);

                var labels = labelString?
                    .Trim([ '[', ']', '"' ])
                    .Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < labels?.Length; i++)
                {
                    labels[i] = labels[i].Trim('\'');
                }

                inputs.Add(new Input
                {
                    Sentence = sentence!,
                    Label = labels!
                });
            }
        }

        return inputs;
    }
}
