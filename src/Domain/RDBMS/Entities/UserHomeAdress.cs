using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.RDBMS.Entities
{
    public class UserHomeAdress : IEntityBase
    {
        public int Id { get; set; }
        public int LocationId { get; set; }
        public string HomeAdress { get; set; }

        public virtual Location Location { get; set; }
        public virtual List<User> User { get; set; }
    }
}
