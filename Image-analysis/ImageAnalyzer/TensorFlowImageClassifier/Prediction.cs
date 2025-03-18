namespace TensorFlowImageClassifier;

public class Prediction : InputData
{
    public float[] Score { get; set; }
    public string PredictedLabelValue { get; set; }
}