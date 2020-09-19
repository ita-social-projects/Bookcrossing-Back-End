using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.RDBMS.Entities
{
    public class Issue : IEntityBase
    {
        public int Id { get; set; }

        public string Name { get; set; }

    }
}
