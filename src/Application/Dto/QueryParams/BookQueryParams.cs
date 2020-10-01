﻿namespace Application.Dto.QueryParams
{
    public class BookQueryParams : PageableParams
    {
        public string SearchTerm { get; set; }

        public int[] Genres { get; set; }

        public int[] Languages { get; set; }

        public int[] Locations { get; set; }

        public int[] HomeLocations { get; set; }

        public bool? ShowAvailable { get; set; }

        public SortableParams SortableParams { get; set; }

        public bool? LocationFilterOn { get; set; }
    }
}
