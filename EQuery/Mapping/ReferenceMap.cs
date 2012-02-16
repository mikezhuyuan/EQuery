using System.Reflection;
using EQuery.Core;

namespace EQuery.Mapping
{
    class ReferenceMap : PropertyMap
    {
        public EntityMap From { get;private set; }
        public EntityMap To { get; private set; }

        public ReferenceMap(PropertyInfo propertyInfo, BracketedName column, EntityMap from, EntityMap to)
            : base(propertyInfo, column)
        {
            From = from;
            To = to;
        }
    }
}