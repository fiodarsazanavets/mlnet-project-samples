namespace NamedEntityRecognition;

public static class LabelsHelper
{
    public static Label[] GetLabels() => [   
        new Label { Key = "PERSON" },       // People
        new Label { Key = "NORP" },         // Demographic groups
        new Label { Key = "FAC" },          // Buildings
        new Label { Key = "ORG" },          // Organizations
        new Label { Key = "CITY" },         // Cities
        new Label { Key = "COUNTRY" },      // Countries
        new Label { Key = "CONTINENT" },    // Continents
        new Label { Key = "LOC" },          // Geographic features
        new Label { Key = "PRODUCT" },      // Objects
        new Label { Key = "EVENT" },        // Events
        new Label { Key = "WORK_OF_ART" },  // Works of art
        new Label { Key = "LAW" },          // Legal documents
        new Label { Key = "LANGUAGE" },     // Languages
        new Label { Key = "DATE" },         // Date
        new Label { Key = "TIME" },         // Time
        new Label { Key = "PERCENT" },      // Percentage
        new Label { Key = "MONEY" },        // Monetary vaue
        new Label { Key = "QUANTITY" },     // Quantity measurements
        new Label { Key = "ORDINAL" },      // Order
        new Label { Key = "CARDINAL" },     // Other numbers
    ];
}
