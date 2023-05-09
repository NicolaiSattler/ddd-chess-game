using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Chess.Domain.Events;
using Chess.Infrastructure.Repository;
using Microsoft.Extensions.Logging;
using Moq;

namespace Chess.Test.Infrastructure;

[TestClass]
public class MatchEventRepositoryTests: TestBase
{
    private MatchEventRepository _sut;
    private Fixture _fixture;
    private Guid _aggregateId = Guid.NewGuid();

    public MatchEventRepositoryTests(): base()
    {
        DbContext.Matches.Add(new()
        {
            AggregateId = _aggregateId
        });
        DbContext.SaveChanges();
    }

    [TestInitialize]
    public void Initialize()
    {
        _sut = new(Mock.Of<ILogger<MatchEventRepository>>(), DbContext);
        _fixture = new();
    }

    [TestMethod]
    public async Task AddAsync_SaveChangesIsFalse_ShouldNotSaveMatch()
    {
        //Arrange
        var matchStarted = _fixture.Create<MatchStarted>();

        //Act
        await _sut.AddAsync(_aggregateId, matchStarted, false);

        //Assert
        DbContext.Events.Any(e => e.AggregateId == _aggregateId).ShouldBeFalse();
    }

    [TestMethod]
    public async Task AddAsync_SaveChangesIsTrue_ShouldSaveMatch()
    {
        //Arrange
        var matchStarted = _fixture.Create<MatchStarted>();

        //Act
        await _sut.AddAsync(_aggregateId, matchStarted);

        //Assert
        DbContext.Events.Any(e => e.AggregateId == _aggregateId).ShouldBeTrue();
    }

    [TestMethod]
    public async Task GetAsync_KnownAggregateId_ShouldReturnEvents()
    {
        //Arrange
        var matchStarted = _fixture.Create<MatchStarted>();

        //Act
        await _sut.AddAsync(_aggregateId, matchStarted, false);
        var result = await _sut.GetAsync(_aggregateId);

        //Assert
        result.ShouldNotBeNull();
    }


    [TestMethod]
    public async Task GetAsync_UnknownAggregateId_ShouldReturnEvents()
    {
        //Arrange
        var matchStarted = _fixture.Create<MatchStarted>();

        //Act
        await _sut.AddAsync(_aggregateId, matchStarted, false);
        var result = await _sut.GetAsync(Guid.NewGuid());

        //Assert
        result.ShouldBeEmpty();
    }

    [TestCleanup]
    public void Cleanup()
    {
        DbContext.Matches.RemoveRange(DbContext.Matches);
        DbContext.Events.RemoveRange(DbContext.Events);
        DbContext.SaveChanges();
    }
}