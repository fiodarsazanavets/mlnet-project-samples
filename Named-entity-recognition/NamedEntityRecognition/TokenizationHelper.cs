using Microsoft.ML.Tokenizers;

namespace NamedEntityRecognition;

public class TokenizationHelper
{
    private static Tokenizer? Instace;
    private static readonly EnglishRoberta Roberta = new("Resources/encoder.json", "Resources/vocab.bpe", "Resources/dict.txt");

    /// <summary>
    /// Tokenize the input text.
    /// </summary>
    public static TokenizerResult Tokenize(string input)
    {
        Roberta.AddMaskSymbol();
        Instace = new Tokenizer(Roberta, new RobertaPreTokenizer());
        return Instace.Encode(input);
    }
}
