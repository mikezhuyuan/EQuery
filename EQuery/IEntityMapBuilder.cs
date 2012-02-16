using System;
using System.Linq.Expressions;

namespace EQuery
{
    public interface IEntityMapBuilder<TEntity> where TEntity : class, new()
    {
        IEntityMapBuilder<TEntity> Table(string table);
        IEntityMapBuilder<TEntity> Schema(string schema);
        IEntityMapBuilder<TEntity> Key<TProperty>(Expression<Func<TEntity, TProperty>> expr, string column);
        IEntityMapBuilder<TEntity> Property<TProperty>(Expression<Func<TEntity, TProperty>> expr, string column);
        IEntityMapBuilder<TEntity> Reference<TProperty>(Expression<Func<TEntity, TProperty>> expr, string foreignKey);
    }
}