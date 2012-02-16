using System;
using System.Reflection;

namespace EQuery.Core
{
    interface INamingConvention : IResolvable
    {
        string GetForeignKeyName(PropertyInfo propertyInfo);
        void GetKey(Type type, out PropertyInfo propertyInfo, out string columnName);
        string GetTableName(Type type);
        string GetColumnName(PropertyInfo propertyInfo);
    }
}