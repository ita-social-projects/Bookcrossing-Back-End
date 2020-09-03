using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.RDBMS.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class NamespaceAttribute: Attribute
    {
        public string Namespace { get; }

        public NamespaceAttribute(string @namespace)
        {
            Namespace = @namespace;
        }
    }
}
