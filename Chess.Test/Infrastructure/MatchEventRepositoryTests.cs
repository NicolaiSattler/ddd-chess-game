using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chess.Domain.Events;
using Chess.Infrastructure.Entity;
using Chess.Infrastructure.Repository;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Chess.Test.Infrastructure;

[TestClass]
public class MatchEventRepositoryTests : TestBase
{
    private MatchEventRepository _sut;
    private Fixture _fixture;
    private Guid _aggregateId = Guid.NewGuid();
    private IMemoryCache _mockedCache;

    [TestInitialize]
    public void Initialize()
    {
        _mockedCache = Substitute.For<IMemoryCache>();
        _sut = new(Substitute.For<ILogger<MatchEventRepository>>(), _mockedCache, DbContext);
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
        var mockedCacheEntry = Substitute.For<ICacheEntry>();
        var outputEntry = new object();

        _mockedCache.CreateEntry(_aggregateId)
                    .Returns(mockedCacheEntry);

        var callCount = 0;
        _mockedCache.TryGetValue(Arg.Any<object>(), out outputEntry)
                    .Returns(_ => callCount++ == 0 ? false : true);

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
        var mockedCacheEntry = Substitute.For<ICacheEntry>();
        var outputEntry = new object();

        _mockedCache.CreateEntry(_aggregateId)
                    .Returns(mockedCacheEntry);

        var callCount = 0;
        _mockedCache.TryGetValue(Arg.Any<object>(), out outputEntry)
                    .Returns(_ => callCount++ == 0 ? false : true);

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
        var output = new List<MatchEvent> { new() { AggregateId = _aggregateId } };

        _mockedCache.TryGetValue(Arg.Any<object>(), out _)
                    .Returns(true);

        //Act
        var result = await _sut.GetAsync(Guid.NewGuid());

        //Assert
        result.ShouldNotBeEmpty();

        _mockedCache.Received(1).TryGetValue(Arg.Any<object>(), out _);
    }

    [TestCleanup]
    public void Cleanup()
    {
        DbContext.Matches.RemoveRange(DbContext.Matches);
        DbContext.Events.RemoveRange(DbContext.Events);
        DbContext.SaveChanges();
    }
}
