using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.APIGatewayEvents;

using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

using Xunit;
using ILV.Api.Domain;
using ILV.Api.Data;

namespace ILV.Api.Tests.Integration
{
    public class FunctionTest : IDisposable
    { 
        string _tableName { get; }
        IAmazonDynamoDB _dDBClient { get; }

        private readonly IMiningService _miningService;
        private readonly Functions sut;

        public FunctionTest()
        {
            _tableName = "ilv-mining-" + DateTime.Now.Ticks;
            _dDBClient = new AmazonDynamoDBClient(RegionEndpoint.APSoutheast2);
            SetupTableAsync().Wait();

            _miningService = new MiningService(new PersistenceService(_dDBClient, _tableName));
            sut = new Functions(_miningService);
        }

        [Fact]
        public async Task APIReturns0WhenRequestingWithin30SecondsOfStartingMining()
        {
            var request = new APIGatewayProxyRequest();
            var context = new TestLambdaContext();

            var response = await sut.StartMiningAsync(request, context);
            Assert.Equal(201, response.StatusCode);

            var miningId = response.Body;
            request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string> { { Functions.ID_QUERY_STRING_NAME, miningId } }
            };
            context = new TestLambdaContext();

            response = await sut.GetMiningStatusAsync(request, context);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal("0", response.Body);
        }

        [Fact]
        public async Task APIReturnsNon0AndDeletesTheResourceWhenRequestingWithin30To90SecondsOfStartingMining()
        {
            var request = new APIGatewayProxyRequest();
            var context = new TestLambdaContext();

            var response = await sut.StartMiningAsync(request, context);
            Assert.Equal(201, response.StatusCode);

            var miningId = response.Body;
            request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string> { { Functions.ID_QUERY_STRING_NAME, miningId } }
            };
            context = new TestLambdaContext();

            Thread.Sleep(TimeSpan.FromSeconds(31));
            response = await sut.GetMiningStatusAsync(request, context);

            Assert.Equal(200, response.StatusCode);
            Assert.NotEqual("0", response.Body);

            request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string> { { Functions.ID_QUERY_STRING_NAME, miningId } }
            };
            context = new TestLambdaContext();

            response = await sut.GetMiningStatusAsync(request, context);

            Assert.Equal(404, response.StatusCode);
        }

        [Fact]
        public async Task APIReturns0WhenRequestingAfters90SecondsOfStartingMining()
        {
            var request = new APIGatewayProxyRequest();
            var context = new TestLambdaContext();

            var response = await sut.StartMiningAsync(request, context);
            Assert.Equal(201, response.StatusCode);

            var miningId = response.Body;
            request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string> { { Functions.ID_QUERY_STRING_NAME, miningId } }
            };
            context = new TestLambdaContext();

            Thread.Sleep(TimeSpan.FromSeconds(90));
            response = await sut.GetMiningStatusAsync(request, context);

            Assert.Equal(200, response.StatusCode);
            Assert.Equal("0", response.Body);
        }

        [Fact]
        public async Task APIReturnsNotFoundWhenRequestingInvalidId()
        {
            var request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string> { { Functions.ID_QUERY_STRING_NAME, "Invalid_ID" } }
            };
            var context = new TestLambdaContext();

            var response = await sut.GetMiningStatusAsync(request, context);

            Assert.Equal(404, response.StatusCode);
        }

        [Fact]
        public async Task APIReturnsBadRequestWhenNotSupplyingId()
        {
            var request = new APIGatewayProxyRequest();
            var context = new TestLambdaContext();

            var response = await sut.GetMiningStatusAsync(request, context);

            Assert.Equal(400, response.StatusCode);
        }

        private async Task SetupTableAsync()
        {
            
            var request = new CreateTableRequest
            {
                TableName = _tableName,
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 2,
                    WriteCapacityUnits = 2
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        KeyType = KeyType.HASH,
                        AttributeName = Functions.ID_QUERY_STRING_NAME
                    }
                },
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition
                    {
                        AttributeName = Functions.ID_QUERY_STRING_NAME,
                        AttributeType = ScalarAttributeType.S
                    }
                }
            };

            await _dDBClient.CreateTableAsync(request);

            var describeRequest = new DescribeTableRequest { TableName = _tableName };
            DescribeTableResponse response;
            do
            {
                Thread.Sleep(1000);
                response = await _dDBClient.DescribeTableAsync(describeRequest);
            } while (response.Table.TableStatus != TableStatus.ACTIVE);
        }


        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _dDBClient.DeleteTableAsync(_tableName).Wait();
                    _dDBClient.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
