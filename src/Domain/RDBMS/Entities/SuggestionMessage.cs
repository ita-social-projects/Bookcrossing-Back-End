
namespace Domain.RDBMS.Entities
{
    public class SuggestionMessage:IEntityBase
    {
        public int Id { get; set; }
        public string Summary { get; set; }
        public string Text { get; set; }
        public MessageState State { get; set; }
        public int UserId { get; set; }
        public  User User { get; set; }
    }
}
