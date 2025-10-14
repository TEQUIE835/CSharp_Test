namespace PruebaDesempenio.Domain.Interfaces;

public interface IUpdate<T>
{
    Task Update(T obj);
}