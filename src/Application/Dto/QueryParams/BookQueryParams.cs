using Domain.RDBMS.Entities;

namespace Application.Dto.QueryParams
{
    public class BookQueryParams : PageableParams
    {
        public string SearchTerm { get; set; }

        public int[] Genres { get; set; }

        public int[] Languages { get; set; }

        public int[] Locations { get; set; }

        public int[] HomeLocations { get; set; }

        public BookState[] BookStates { get; set; }

        public SortableParams SortableParams { get; set; }

        public bool? LocationFilterOn { get; set; }
    }
}
