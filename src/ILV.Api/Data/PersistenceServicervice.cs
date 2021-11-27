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
        const string TABLENAME_ENVIRONMENT_VARIABLE_LOOKUP = "MiningTable";

        public PersistenceService()
        {
            var tableName = Environment.GetEnvironmentVariable(TABLENAME_ENVIRONMENT_VARIABLE_LOOKUP);
            SetDbContext(new AmazonDynamoDBClient(), tableName);
        }

        public PersistenceService(IAmazonDynamoDB ddbClient, string tableName)
        {
            SetDbContext(ddbClient, tableName);
        }

        public async Task SaveAsync(Mining mining)
        {
            await _dDBContext.SaveAsync<Mining>(mining);
        }

        public async Task<Mining> GetAsync(string id)
        {
            return await _dDBContext.LoadAsync<Mining>(id);
        }

        public async Task DeleteAsync(string id)
        {
            await _dDBContext.DeleteAsync<Mining>(id);
        }

        private void SetDbContext(IAmazonDynamoDB ddbClient, string tableName)
        {
            AWSConfigsDynamoDB.Context.TypeMappings[typeof(Mining)] = new Amazon.Util.TypeMapping(typeof(Mining), tableName);
            var config = new DynamoDBContextConfig { Conversion = DynamoDBEntryConversion.V2 };
            _dDBContext = new DynamoDBContext(ddbClient, config);
        }
    }
}
