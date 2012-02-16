using System;
using System.Reflection;
using EQuery.Core;

namespace EQuery.Mapping
{
    class EntityMap
    {
        public Type Type;
        public BracketedName Schema;
        public BracketedName Table;
        public PropertyMap Key;
        public PropertyMap[] Properties;
        public ReferenceMap[] References;

        public Delegate Assemble;

        public PropertyMap FindPropertyMap(PropertyInfo propertyInfo)
        {
            if (Properties != null)
                foreach (var mapping in Properties)
                    if (mapping.Property == propertyInfo)
                        return mapping;

            return null;
        }

        public ReferenceMap FindReferenceMap(PropertyInfo propertyInfo)
        {
            if (Properties != null)
                foreach (var mapping in References)
                    if (mapping.Property == propertyInfo)
                        return mapping;

            return null;
        }
    }
}