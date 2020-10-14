
using System;
using Microsoft.ML.Data;

namespace Application.Dto
{
    public class ModelOutputDto
    {
        [ColumnName("PredictedLabel")]
        public String Prediction { get; set; }
        public float[] Score { get; set; }
    }
}
