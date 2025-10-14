namespace PruebaDesempenio.Domain.Interfaces;

public interface ICreate<T>
{
   Task Create(T obj);
}