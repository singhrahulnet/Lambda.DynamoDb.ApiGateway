using System;
using Xunit;
using Moq;
using ILV.Api.Domain;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using System.Collections.Generic;

namespace ILV.Api.Tests.Unit
{
    public class FunctionsTests
    {
        private readonly Mock<IMiningService> _mockMiningService;
        private readonly Functions sut;

        public FunctionsTests()
        {
            _mockMiningService = new Mock<IMiningService>();
            sut = new Functions(_mockMiningService.Object);
        }

        [Fact]
        public async Task GivenMiningServiceReturnsANewlyAddedItem_WhenCallingPostLambda_ThenNewGUIDIsReturned()
        {
            var newlyAddedGuid = Guid.NewGuid();
            _mockMiningService.Setup(x=>x.StartMining())
                                .Returns(Task.FromResult<Guid>(newlyAddedGuid));

            var result = await sut.AddNFTAsync(null, null);

            Assert.IsType<APIGatewayProxyResponse>(result);
            Assert.Equal(201, result.StatusCode);
            Assert.Equal(newlyAddedGuid.ToString(), result.Body);
        }

        [Fact]
        public async Task GivenTheIdIsPassedAsNull_WhenCallingGetLambda_ThenBadRequestIsReturned()
        {

            var request = new APIGatewayProxyRequest();

            var result = await sut.GetNFTAsync(request, null);

            Assert.IsType<APIGatewayProxyResponse>(result);
            Assert.Equal(400, result.StatusCode);
        }

        [Fact]
        public async Task GivenMiningServiceReturnsAPositiveNumber_WhenCallingGetLambda_ThenThatNumberIsReturnedWith200()
        {
            var newlyAddedGuid = Guid.NewGuid().ToString();

            var request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string> { { Functions.ID_QUERY_STRING_NAME, newlyAddedGuid } }
            };

            _mockMiningService.Setup(x => x.GetMiningResult(newlyAddedGuid))
                                .Returns(Task.FromResult<int>(99));


            var result = await sut.GetNFTAsync(request, null);

            Assert.IsType<APIGatewayProxyResponse>(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal("99", result.Body);
        }

        [Fact]
        public async Task GivenMiningServiceReturnsMinus1_WhenCallingGetLambda_404IsReturned()
        {
            var newlyAddedGuid = Guid.NewGuid().ToString();

            var request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string> { { Functions.ID_QUERY_STRING_NAME, newlyAddedGuid } }
            };

            _mockMiningService.Setup(x => x.GetMiningResult(newlyAddedGuid))
                                .Returns(Task.FromResult<int>(-1));


            var result = await sut.GetNFTAsync(request, null);

            Assert.IsType<APIGatewayProxyResponse>(result);
            Assert.Equal(404, result.StatusCode);
        }
    }
}
