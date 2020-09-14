using System.Collections.Generic;

namespace Application.Dto
{
    public class StatisticsChartData
    {
        public List<string> Periods { get; set; }

        public Dictionary<string, List<int>> Data { get; set; }

        public StatisticsChartData(List<string> periods, Dictionary<string, List<int>> data)
        {
            Periods = periods;
            Data = data;
        }
    }
}
