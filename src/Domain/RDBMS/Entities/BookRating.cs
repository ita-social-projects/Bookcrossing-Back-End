namespace Domain.RDBMS.Entities
{
    public class BookRating : IEntityBase
    {
        public BookRating(int bookId, int userId, double rating)
        {
            BookId = bookId;
            UserId = userId;
            Rating = rating;
        }

        public int BookId { get; set; }
        public int UserId { get; set; }
        public double Rating { get; set; }

        public virtual Book Book { get; set; }
        public virtual User User { get; set; }
    }
}
