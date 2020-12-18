using System.Threading.Tasks;

namespace Kola.Infrastructure.Config.Contract
{
    public interface IJsonLoader<T>
    {
        Task<T> GetAsync(string penvironment);
    }
}
