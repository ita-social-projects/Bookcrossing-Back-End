using Application.Dto;
using Application.Services.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Implementation
{
    public class SentimentAnalysisService : ISentimentAnalisysService
    {
        private const string _dummyLabel = "0";
        private PredictionEnginePool<ModelInputDto, ModelOutputDto> _enginePool;
        public SentimentAnalysisService(PredictionEnginePool<ModelInputDto, ModelOutputDto> enginePool)
        {
            _enginePool = enginePool;
        }

        private float ConvertToStar(ModelOutputDto modelOutputDto)
        {
            return modelOutputDto.Score.LastOrDefault() * 5;
        }
        public async Task<float> Predict(string text)
        {
            var model = new ModelInputDto()
            {
                Text = text,
                Label = _dummyLabel
            };

            var engine = _enginePool.GetPredictionEngine("SentimentAnalysisModel");
            var predictionModel = await Task.Run(() => engine.Predict(model));

            var starPrediction = ConvertToStar(predictionModel);
            return starPrediction;
        }
    }
}
