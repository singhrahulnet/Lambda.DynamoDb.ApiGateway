using System;
using System.Threading.Tasks;
using ILV.Api.Data;
using ILV.Api.Models;

namespace ILV.Api.Domain
{
    public class MiningService : IMiningService
    {
        IPersistenceService _persistenceService;

        public MiningService(IPersistenceService persistenceService)
        {
            _persistenceService = persistenceService;
        }

        public async Task<Guid> StartMining()
        {
            var result = Guid.NewGuid();
            var nft = new NFT { Id = result.ToString(), CreatedTimestamp = DateTime.UtcNow };

            await _persistenceService.SaveAsync(nft);

            return result;
        }

        public async Task<int> GetMiningResult(string id)
        {
            int result = 0;

            var nft = await _persistenceService.GetAsync(id);

            if (nft == null) result = -1;

            else if (NFTCreatedTimeIsInRange(nft.CreatedTimestamp, 30, 90))
            {
                await _persistenceService.DeleteAsync(id);
                result = new Random().Next(1, 99);
            }

            return result;
        }

        private bool NFTCreatedTimeIsInRange(DateTime input, int lowerbound, int upperBound)
        {
            var delta = new TimeSpan(DateTime.Now.Ticks - input.Ticks).TotalSeconds;
            return delta >= lowerbound && delta <= upperBound;
        }
    }
}
