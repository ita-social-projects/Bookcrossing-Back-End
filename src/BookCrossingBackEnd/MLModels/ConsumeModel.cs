// This file was auto-generated by ML.NET Model Builder. 

using System;
using System.IO;
//using Microsoft.ML;
using Application.Dto;

//namespace Application.MLModels
//{
//    public class ConsumeModel
//    {
//        private static Lazy<PredictionEngine<ModelInputDto, ModelOutputDto>> PredictionEngine = new Lazy<PredictionEngine<ModelInputDto, ModelOutputDto>>(CreatePredictionEngine);

//        public static string MLNetModelPath = Path.GetFullPath("MLModel.zip");

//        public static ModelOutputDto Predict(ModelInputDto input)
//        {
//            ModelOutputDto result = PredictionEngine.Value.Predict(input);
//            return result;
//        }

//        public static PredictionEngine<ModelInputDto, ModelOutputDto> CreatePredictionEngine()
//        {
//            // Create new MLContext
//            MLContext mlContext = new MLContext();

//            // Load model & create prediction engine
//            ITransformer mlModel = mlContext.Model.Load(MLNetModelPath, out var modelInputSchema);
//            var predEngine = mlContext.Model.CreatePredictionEngine<ModelInputDto, ModelOutputDto>(mlModel);

//            return predEngine;
//        }
//    }
//}
