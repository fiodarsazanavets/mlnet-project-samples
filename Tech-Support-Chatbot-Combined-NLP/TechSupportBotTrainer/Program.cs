using Microsoft.ML;
using TechSupportBotTrainer.Trainers;

const string BasePathToDataFolder = "C:\\Repos\\machine-learning-data-samples\\Supervised\\Combined-NLP";
const string ModelsBasePath = "C:\\Temp";

var ml = new MLContext(seed: 1);

ml.TrainTextClassificationModel(BasePathToDataFolder, ModelsBasePath);
ml.TrainSentenceSimilarityModel(BasePathToDataFolder, ModelsBasePath);
ml.TrainNamedEntityRecognitionModel(BasePathToDataFolder, ModelsBasePath);
ml.TrainQuestionAnsweringModel(BasePathToDataFolder, ModelsBasePath);