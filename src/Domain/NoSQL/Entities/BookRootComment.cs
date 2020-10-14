using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.NoSQL.Entities
{
    [BsonIgnoreExtraElements]
    public class BookRootComment : IRootEntityBase, IBookComment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfNull]
        public string Id { get; set; }

        [BsonIgnoreIfNull]
        public string Text { get; set; }

        [BsonIgnoreIfNull]
        public string Date { get; set; }

        [BsonIgnoreIfDefault]
        public int BookId { get; set; }

        [BsonIgnoreIfDefault]
        public int OwnerId { get; set; }

        public bool IsDeleted { get; set; } = false;

        [BsonIgnoreIfNull]
        public IEnumerable<BookChildComment> Comments { get; set; }

        public float PredictedRating { get; set; }

        public BookRootComment() { }

        public BookRootComment(bool IsForInserting)
        {
            if (IsForInserting)
            {
                Id = ObjectId.GenerateNewId().ToString();
                Comments = new List<BookChildComment>();
            }
        }
    }
}
