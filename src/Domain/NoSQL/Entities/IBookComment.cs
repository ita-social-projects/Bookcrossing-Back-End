using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.NoSQL.Entities
{
    /// <summary>
    /// Used for calculation of avarage PredictedRating recurcively
    /// </summary>
    public interface IBookComment
    {
        float PredictedRating { get; set; }
        IEnumerable<BookChildComment> Comments { get; set; }
        public bool IsDeleted { get; set; }
    }
}
