using System;

namespace Domain.RDBMS.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class NamespaceAttribute : Attribute
    {
        public string Namespace { get; }

        public NamespaceAttribute(string @namespace)
        {
            if (string.IsNullOrWhiteSpace(@namespace))
            {
                throw new ArgumentException("Namespace value cannot be null or white spaces", nameof(@namespace));
            }

            Namespace = @namespace;
        }
    }
}
