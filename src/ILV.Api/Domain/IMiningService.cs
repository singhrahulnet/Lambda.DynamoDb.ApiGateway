using System;
using System.Threading.Tasks;

namespace ILV.Api.Domain
{
    public interface IMiningService
    {
        Task<Guid> StartMining();
        Task<int> GetMiningResult(string id);
    }
}
