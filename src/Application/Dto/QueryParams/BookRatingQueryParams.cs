namespace Application.Dto.QueryParams
{
    public class BookRatingQueryParams
    {
        public int BookId { get; set; }
        public int UserId { get; set; }
        public double Rating { get; set; }
    }
}
