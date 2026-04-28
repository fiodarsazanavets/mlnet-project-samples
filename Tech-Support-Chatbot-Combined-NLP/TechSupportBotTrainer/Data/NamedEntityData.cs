namespace TechSupportBotTrainer.Data;

public sealed class NamedEntityData
{
    public string Sentence { get; set; } = string.Empty;
    public string[] Label { get; set; } = [];
    public string[] OriginalLabel { get; set; } = [];
}
