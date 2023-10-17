using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Chess.Domain.Events;
using Chess.Infrastructure.Entity;
using Chess.Infrastructure.Repository;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace Chess.Test.Infrastructure;

[TestClass]
public class MatchEventRepositoryTests: TestBase
{
    private MatchEventRepository _sut;
    private Fixture _fixture;
    private Guid _aggregateId = Guid.NewGuid();
    private Mock<IMemoryCache> _mockedCache;

    [TestInitialize]
    public void Initialize()
    {
        _mockedCache = new();
        _sut = new(Mock.Of<ILogger<MatchEventRepository>>(), _mockedCache.Object, DbContext);
        _fixture = new();

        DbContext.Matches.Add(new()
        {
            AggregateId = _aggregateId,
            Options = new()
        });
        DbContext.SaveChanges();
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
        var mockedCacheEntry = Mock.Of<ICacheEntry>();
        var outputEntry = new object();

        _mockedCache.Setup(m => m.CreateEntry(_aggregateId))
                    .Returns(mockedCacheEntry)
                    .Verifiable();

        _mockedCache.SetupSequence(m => m.TryGetValue(It.IsAny<object>(), out outputEntry))
                    .Returns(false)
                    .Returns(true);

        //Act
        await _sut.AddAsync(_aggregateId, matchStarted, true);
        var result = await _sut.GetAsync(_aggregateId);

        //Assert
        result.ShouldNotBeNull();
    }


    [TestMethod]
    public async Task GetAsync_UnknownAggregateId_ShouldReturnEvents()
    {
        //Arrange
        var matchStarted = _fixture.Create<MatchStarted>();
        var mockedCacheEntry = Mock.Of<ICacheEntry>();
        var outputEntry = new object();

        _mockedCache.Setup(m => m.CreateEntry(_aggregateId))
                    .Returns(mockedCacheEntry)
                    .Verifiable();

        _mockedCache.SetupSequence(m => m.TryGetValue(It.IsAny<object>(), out outputEntry))
                    .Returns(false)
                    .Returns(true);

        //Act
        await _sut.AddAsync(_aggregateId, matchStarted, true);
        var result = await _sut.GetAsync(Guid.NewGuid());

        //Assert
        result.ShouldBeEmpty();
    }


    [TestMethod]
    public async Task GetAsync_ShouldReturnCachedEvents()
    {
        //Arrange
        var output = new List<MatchEvent> { new MatchEvent() { AggregateId = _aggregateId } };
        object outputEntry = output;

        _mockedCache.Setup(m => m.TryGetValue(It.IsAny<object>(), out outputEntry))
                    .Returns(true)
                    .Verifiable();

        //Act
        var result = await _sut.GetAsync(Guid.NewGuid());

        //Assert
        result.ShouldNotBeEmpty();
        _mockedCache.Verify();
    }
    [TestCleanup]
    public void Cleanup()
    {
        DbContext.Matches.RemoveRange(DbContext.Matches);
        DbContext.Events.RemoveRange(DbContext.Events);
        DbContext.SaveChanges();
    }
}