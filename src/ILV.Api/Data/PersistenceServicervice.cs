using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using ILV.Api.Models;

namespace ILV.Api.Data
{
    public class PersistenceService : IPersistenceService
    {
        private IDynamoDBContext _dDBContext { get; set; }
        const string TABLENAME_ENVIRONMENT_VARIABLE_LOOKUP = "NFTTable";

        public PersistenceService()
        {
            var tableName = Environment.GetEnvironmentVariable(TABLENAME_ENVIRONMENT_VARIABLE_LOOKUP);
            SetDbContext(new AmazonDynamoDBClient(), tableName);
        }

        public PersistenceService(IAmazonDynamoDB ddbClient, string tableName)
        {
            SetDbContext(ddbClient, tableName);
        }

        private void SetDbContext(IAmazonDynamoDB ddbClient, string tableName)
        {
            AWSConfigsDynamoDB.Context.TypeMappings[typeof(NFT)] = new Amazon.Util.TypeMapping(typeof(NFT), tableName);
            var config = new DynamoDBContextConfig { Conversion = DynamoDBEntryConversion.V2 };
            _dDBContext = new DynamoDBContext(ddbClient, config);
        }

        public async Task SaveAsync(NFT nft)
        {
            await _dDBContext.SaveAsync<NFT>(nft);
        }

        public async Task<NFT> GetAsync(string id)
        {
            return await _dDBContext.LoadAsync<NFT>(id);
        }

        public async Task DeleteAsync(string id)
        {
            await _dDBContext.DeleteAsync<NFT>(id);
        }
    }
}
