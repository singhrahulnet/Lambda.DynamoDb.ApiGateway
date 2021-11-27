using System.Threading.Tasks;
using ILV.Api.Models;

namespace ILV.Api.Data
{
    public interface IPersistenceService
    {
        Task SaveAsync(Mining mining);
        Task<Mining> GetAsync(string id);
        Task DeleteAsync(string id);
    }
}
