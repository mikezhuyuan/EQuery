using EQuery.Core;
using EQuery.Mapping;

namespace EQuery.Sql.SqlNode
{
    class PropertyAccess : IExprNode
    {
        public BracketedName Alias;
        public PropertyMap PropertyMap;
        public IExprNode Parent;

        public void Render(SqlWriter writer)
        {
            var type = PropertyMap.Property.PropertyType;
            if (type == typeof(bool) && !(Parent is Relational))
            {
                writer.Append(Alias + PropertyMap.Column, SqlStrings.Equal, "1");  
            }
            else
            {
                writer.Append(Alias + PropertyMap.Column);    
            }            
        }

        public Precedence Precedence
        {
            get { return Precedence.Hightest; }
        }
    }
}