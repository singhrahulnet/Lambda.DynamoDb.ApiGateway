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
        public async Task GivenTheDbCallWorks_WhenCallingStartMining_ThenGuidIsReturned()
        {
            _mockPersisytenceService.Setup(x => x.SaveAsync(It.IsAny<Mining>()))
                                        .Returns(Task.CompletedTask);

            var result = await sut.StartMining();

            Assert.IsType<Guid>(result);
        }

        [Fact]
        public async Task GivenTheIdIsNotInDb_WhenCallingGetMiningResult_ThenMiningResultNotFoundExceptionIsThrown()
        {
            string id = "randomId";

            _mockPersisytenceService.Setup(x => x.GetAsync(id))
                                        .Returns(Task.FromResult<Mining>(null));

            await Assert.ThrowsAsync<MiningResultNotFoundException>(() => sut.GetMiningResult(id));
        }

        [Fact]
        public async Task GivenTheItemHasTimeStampWithin30To90Seconds_WhenCallingGetMiningResult_ThenANumberBetween1And99IsReturned()
        {
            DateTime time30SecondsAgo = DateTime.Now.AddSeconds(-30);
            var mining = new Mining { Id = Guid.NewGuid().ToString(), CreatedTimestamp = time30SecondsAgo };

            _mockPersisytenceService.Setup(x => x.GetAsync(mining.Id))
                                        .Returns(Task.FromResult<Mining>(mining));

            var result = await sut.GetMiningResult(mining.Id);

            Assert.InRange<int>(result, 1, 99);
            _mockPersisytenceService.Verify(x => x.DeleteAsync(mining.Id), Times.Once);
        }

        [Fact]
        public async Task GivenTheItemHasTimeStampWithinLessThan30SecondsAgo_WhenCallingGetMiningResult_Then0IsReturned()
        {
            DateTime timeLessThan30Seconds = DateTime.Now.AddSeconds(-20);
            var mining = new Mining { Id = Guid.NewGuid().ToString(), CreatedTimestamp = timeLessThan30Seconds };

            _mockPersisytenceService.Setup(x => x.GetAsync(mining.Id))
                                        .Returns(Task.FromResult<Mining>(mining));

            var result = await sut.GetMiningResult(mining.Id);

            Assert.Equal(0, result);
        }

        [Fact]
        public async Task GivenTheItemHasTimeStampMoreThan90SecondsAgo_WhenCallingGetMiningResult_Then0IsReturned()
        {
            DateTime timeMoreThan90SecondsAgo = DateTime.Now.AddSeconds(-100);
            var mining = new Mining { Id = Guid.NewGuid().ToString(), CreatedTimestamp = timeMoreThan90SecondsAgo };

            _mockPersisytenceService.Setup(x => x.GetAsync(mining.Id))
                                        .Returns(Task.FromResult<Mining>(mining));

            var result = await sut.GetMiningResult(mining.Id);

            Assert.Equal(0, result);
        }
    }
}
