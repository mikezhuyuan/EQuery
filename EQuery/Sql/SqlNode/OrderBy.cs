using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EQuery.Core;
using EQuery.Mapping;

namespace EQuery.Sql.SqlNode
{
    class OrderBy : ISqlNode
    {
        public void AddItem(BracketedName alias, PropertyMap property, bool ascending)
        {
            _items.Add(new Item
            {
                Alias = alias,
                Property = property,
                Ascending = ascending,
            });
        }

        public void Render(SqlWriter writer)
        {                                    
            for (var i = 0; i < _items.Count; i++)
            {
                var item = _items[i];

                if(i == 0)
                    writer.Append(SqlStrings.OrderBy);
                else
                    writer.Append(",");

                writer.Append(item.Alias + item.Property.Column,
                              item.Ascending ? SqlStrings.Ascending : SqlStrings.Descending);
            }
        }

        private readonly IList<Item> _items = new List<Item>();
        private class Item
        {
            public BracketedName Alias;
            public PropertyMap Property;
            public bool Ascending; 
        }
    }
}