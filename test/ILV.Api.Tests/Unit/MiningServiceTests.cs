using System;
using System.Threading.Tasks;
using ILV.Api.Data;
using ILV.Api.Domain;
using ILV.Api.Models;
using Moq;
using Xunit;

namespace ILV.Api.Tests.Unit
{
    public class MiningServiceTests
    {
        private readonly Mock<IPersistenceService> _mockPersisytenceService;
        private readonly MiningService sut;

        public MiningServiceTests()
        {
            _mockPersisytenceService = new Mock<IPersistenceService>();
            sut = new MiningService(_mockPersisytenceService.Object);
        }

        [Fact]
        public async Task Given_Db_Call_Works_When_StartMining_IS_Called_Then_MiningService_Returns_New_GUID()
        {
            _mockPersisytenceService.Setup(x => x.SaveAsync(It.IsAny<NFT>())).Returns(Task.CompletedTask);

            var result = await sut.StartMining();

            Assert.IsType<Guid>(result);
        }
    }
}
