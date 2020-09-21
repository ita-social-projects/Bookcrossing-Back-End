using Domain.RDBMS.Enums;

namespace Domain.RDBMS.Entities
{
    public class Setting : IEntityBase
    {
        public const string DefaultNamespace = "Global";

        public string Namespace { get; set; }

        public SettingKey Key { get; set; }

        public string Value { get; set; }

        public string Description { get; set; }
    }
}
