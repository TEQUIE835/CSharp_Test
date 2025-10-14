namespace PruebaDesempenio.Domain.Interfaces;

public interface IGetAll<T>
{
    Task<IEnumerable<T>> GetAll();
}