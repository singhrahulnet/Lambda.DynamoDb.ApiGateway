using System.Threading.Tasks;
using ILV.Api.Models;

namespace ILV.Api.Data
{
    public interface IPersistenceService
    {
        Task SaveAsync(NFT nft);
        Task<NFT> GetAsync(string id);
        Task DeleteAsync(string id);
    }
}
