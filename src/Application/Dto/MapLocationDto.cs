namespace Application.Dto
{
    public class MapLocationDto
    {
        public LocationDto Location { get; set; }
        public int BooksQuantity { get; set; }

        public MapLocationDto(LocationDto location, int booksQuantity)
        {
            Location = location;
            BooksQuantity = booksQuantity;
        }
    }
}
