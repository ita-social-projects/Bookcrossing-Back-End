using System;

namespace Domain.RDBMS.Entities
{
    public class ResetPassword : IEntityBase
    {
        public int Id { get; set; }
        public string ConfirmationNumber { get; set; }
        public DateTime ResetDate { get; set; }
    }
}
