using System.Collections.Generic;

namespace Application.Dto.Statistics
{
    public class PieChartData
    {
        public PieChartData()
        {
        }

        public PieChartData(int total, IDictionary<string, int> data)
        {
            Total = total;
            Data = data;
        }

        public int Total { get; set; }
        public IDictionary<string, int> Data { get; set; }
    }
}
