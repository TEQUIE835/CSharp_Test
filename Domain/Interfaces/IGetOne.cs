namespace PruebaDesempenio.Domain.Interfaces;

public interface IGetOne<T>
{
    Task<T> GetOne(int id);
}