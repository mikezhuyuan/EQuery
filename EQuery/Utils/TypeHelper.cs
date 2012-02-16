using System;
using System.Reflection;

namespace EQuery.Utils
{
    static class TypeHelper
    {
        public static bool IsEntityProperty(this PropertyInfo propertyInfo)
        {
            return propertyInfo.CanRead
                   && propertyInfo.CanWrite
                   && IsSqlSupportedType(propertyInfo.PropertyType)
                   && propertyInfo.GetSetMethod(false) != null;
        }

        public static bool IsEntityReference(this PropertyInfo propertyInfo)
        {
            return propertyInfo.CanRead
                   && propertyInfo.CanWrite
                   && !IsSqlSupportedType(propertyInfo.PropertyType)
                   && propertyInfo.GetSetMethod(false) != null;
        }

        public static bool IsSqlSupportedType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                return IsSqlSupportedType(Nullable.GetUnderlyingType(type));

            return type.IsPrimitive || type == typeof(string) || type == typeof(DateTime);
        }

        public static bool IsGenericType(this Type type, Type genericType)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().Equals(genericType);
        }
    }
}
