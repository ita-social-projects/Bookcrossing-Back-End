using Application.Dto;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface ISentimentAnalisysService
    {
        Task<float> Predict(string test);
    }
}