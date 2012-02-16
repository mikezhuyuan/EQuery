using System;
using System.Linq;
using System.Reflection;

namespace EQuery.Core
{
    class DefaultConvention : INamingConvention
    {
        public string GetForeignKeyName(PropertyInfo propertyInfo)
        {
            var name = propertyInfo.Name;
            if (!name.EndsWith("_id"))
                return propertyInfo.Name + "_id";

            return name;
        }

        public void GetKey(Type type, out PropertyInfo propertyInfo, out string columnName)
        {
            var lookingFor = new [] {type.Name + "_id", type.Name + "Id", "ID"};

            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach(var prop in props)
            {
                if(prop.CanRead 
                   && prop.CanWrite 
                   && prop.GetSetMethod(false) != null)
                {
                    if (lookingFor.Any(_ => _.Equals(prop.Name, StringComparison.OrdinalIgnoreCase)))
                    {
                        propertyInfo = prop;
                        columnName = prop.Name;
                        return;
                    }                    
                }
            }
            
            throw new Exception("Cannot find key in " + type);
        }

        public string GetTableName(Type type)
        {
            return type.Name;
        }

        public string GetColumnName(PropertyInfo propertyInfo)
        {
            return propertyInfo.Name;
        }
    }
}