using System;

namespace Application.Dto.Statistics
{
    public class StatisticsQueryParams
    {
        public string language { get; set; } = "en";

        public string[] Cities { get; set; }

        public string[] Offices { get; set; }

        public int[] Genres { get; set; }

        public DateTime? From { get; set; }

        public DateTime? To { get; set; }
    }
}
