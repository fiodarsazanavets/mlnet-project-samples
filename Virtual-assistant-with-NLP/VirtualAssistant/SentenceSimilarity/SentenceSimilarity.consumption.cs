﻿// This file was auto-generated by ML.NET Model Builder.
using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.IO;
namespace SentenceSimilarity
{
    public partial class SentenceSimilarity
    {
        /// <summary>
        /// model input class for SentenceSimilarity.
        /// </summary>
        #region model input class
        public class ModelInput
        {
            [LoadColumn(0)]
            [ColumnName(@"Sentence1")]
            public string Sentence1 { get; set; }

            [LoadColumn(1)]
            [ColumnName(@"Sentence2")]
            public string Sentence2 { get; set; }

            [LoadColumn(2)]
            [ColumnName(@"Similarity")]
            public float Similarity { get; set; }

        }

        #endregion

        /// <summary>
        /// model output class for SentenceSimilarity.
        /// </summary>
        #region model output class
        public class ModelOutput
        {
            [ColumnName(@"Sentence1")]
            public string Sentence1 { get; set; }

            [ColumnName(@"Sentence2")]
            public string Sentence2 { get; set; }

            [ColumnName(@"Similarity")]
            public float Similarity { get; set; }

            [ColumnName(@"Score")]
            public float Score { get; set; }

        }

        #endregion

        private static string MLNetModelPath = Path.GetFullPath("SentenceSimilarity.mlnet");

        public static readonly Lazy<PredictionEngine<ModelInput, ModelOutput>> PredictEngine = new Lazy<PredictionEngine<ModelInput, ModelOutput>>(() => CreatePredictEngine(), true);


        private static PredictionEngine<ModelInput, ModelOutput> CreatePredictEngine()
        {
            var mlContext = new MLContext();
            ITransformer mlModel = mlContext.Model.Load(MLNetModelPath, out var _);
            return mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);
        }

        /// <summary>
        /// Use this method to predict on <see cref="ModelInput"/>.
        /// </summary>
        /// <param name="input">model input.</param>
        /// <returns><seealso cref=" ModelOutput"/></returns>
        public static ModelOutput Predict(ModelInput input)
        {
            var predEngine = PredictEngine.Value;
            return predEngine.Predict(input);
        }
    }
}