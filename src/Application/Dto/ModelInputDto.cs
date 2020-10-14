
using Microsoft.ML.Data;

namespace Application.Dto
{
    public class ModelInputDto
    {
        [ColumnName("text"), LoadColumn(0)]
        public string Text { get; set; }


        [ColumnName("label"), LoadColumn(1)]
        public string Label { get; set; }
    }
}
