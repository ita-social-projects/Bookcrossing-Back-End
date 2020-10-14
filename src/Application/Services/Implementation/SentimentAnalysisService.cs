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
        private PredictionEnginePool<ModelInputDto, ModelOutputDto> _engine;
        public SentimentAnalysisService(PredictionEnginePool<ModelInputDto, ModelOutputDto> engine)
        {
            _engine = engine;
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

            var predictionModel = await Task.Run(() => _engine.Predict("SentimentAnalysisModel", model));

            var starPrediction = ConvertToStar(predictionModel);
            return starPrediction;
        }
    }
}
