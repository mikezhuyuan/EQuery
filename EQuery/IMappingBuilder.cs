namespace EQuery
{
    public interface IMappingBuilder
    {
        IEntityMapBuilder<T> Entity<T>() where T : class, new();
    }
}