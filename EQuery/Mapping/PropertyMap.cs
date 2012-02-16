using System.Reflection;
using EQuery.Core;

namespace EQuery.Mapping
{
    class PropertyMap
    {
        public PropertyInfo Property { get; private set; }
        //public SqlDbType SqlDbType;
        public BracketedName Column { get; private set; }

        public PropertyMap(PropertyInfo propertyInfo, BracketedName column)
        {
            Property = propertyInfo;
            Column = column;
        }
    }
}