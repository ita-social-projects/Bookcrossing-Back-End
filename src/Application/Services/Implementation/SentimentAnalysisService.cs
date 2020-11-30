using Application.Dto;
using Application.Services.Interfaces;
using Microsoft.Extensions.ML;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.Implementation
{
    public class SentimentAnalysisService : ISentimentAnalisysService
    {
        private const string _dummyLabel = "0";
        readonly PredictionEnginePool<ModelInputDto, ModelOutputDto> _enginePool;
        public SentimentAnalysisService(PredictionEnginePool<ModelInputDto, ModelOutputDto> enginePool)
        {
            _enginePool = enginePool;
        }

        private float ConvertToStar(ModelOutputDto modelOutputDto)
        {
            return modelOutputDto.Score.LastOrDefault() * 5;
        }
        public async Task<float> Predict(string test)
        {
            var model = new ModelInputDto()
            {
                Text = test,
                Label = _dummyLabel
            };

            var engine = _enginePool.GetPredictionEngine("SentimentAnalysisModel");
            var predictionModel = await Task.Run(() => engine.Predict(model));

            var starPrediction = ConvertToStar(predictionModel);
            return starPrediction;
        }
    }
}
