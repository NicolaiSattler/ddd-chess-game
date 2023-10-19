using System;
using System.Threading.Tasks;
using Chess.Application.Services;
using Chess.Domain.Commands;
using Chess.Domain.Events;
using Chess.Infrastructure.Repository;

namespace Chess.Test.Application;

[TestClass]
public class PlayerActionServiceTests
{
    private PlayerActionService _sut;
    private IMatchDataService _mockedDataService;
    private IMatchRepository _mockedMatchRepository;
    private IMatchEventRepository _mockedEventRepository;
    private ITimerService _mockedTimerService;
    private Fixture _fixture;

    [TestInitialize]
    public void Initialize()
    {
        _mockedDataService = Substitute.For<IMatchDataService>();
        _mockedMatchRepository = Substitute.For<IMatchRepository>();
        _mockedEventRepository = Substitute.For<IMatchEventRepository>();
        _mockedTimerService = Substitute.For<ITimerService>();
        _fixture = new();
        _sut = new(_mockedDataService, _mockedMatchRepository, _mockedEventRepository, _mockedTimerService);
    }

    [TestMethod]
    public async Task StartMatch_ShouldSaveMatchAndMatchEvent()
    {
        //Arrange
        var aggregateId = Guid.NewGuid();
        var command = new StartMatch
        {
            AggregateId = aggregateId,
            MemberOne = _fixture.Create<Player>(),
            MemberTwo = _fixture.Create<Player>()
        };

        //Act
        await _sut.StartMatchAsync(command);

        //Assert
        await _mockedMatchRepository.Received(1)
                                    .AddAsync(Arg.Is<MatchStarted>(m => m.AggregateId == aggregateId));

        await _mockedEventRepository.Received(1)
                                    .AddAsync(aggregateId, Arg.Is<MatchStarted>(m => m.AggregateId == aggregateId), true);
    }

}