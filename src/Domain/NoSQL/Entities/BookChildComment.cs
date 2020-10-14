using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.NoSQL.Entities
{
    [BsonIgnoreExtraElements]
    public class BookChildComment : IChildEntityBase, IBookComment
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
        public int OwnerId { get; set; }

        public bool IsDeleted { get; set; } = false;

        public IEnumerable<BookChildComment> Comments { get; set; }

        public BookChildComment() { }

        public float PredictedRating { get; set; }

        public BookChildComment(bool IsForInserting)
        {
            if (IsForInserting)
            {
                Id = ObjectId.GenerateNewId().ToString();
                Comments = new List<BookChildComment>();
            }
        }
    }
}
