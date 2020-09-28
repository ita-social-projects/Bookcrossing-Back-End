namespace Application.Dto
{
    public class MapHomeLocationDto
    {
        public LocationHomeDto Location { get; set; }
        public int BooksQuantity { get; set; }

        public MapHomeLocationDto(LocationHomeDto location, int booksQuantity)
        {
            Location = location;
            BooksQuantity = booksQuantity;
        }
    }
}
