using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Chess.Domain.Events;
using Chess.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Chess.Test.Infrastructure;

[TestClass]
public class MatchRepositoryTests: TestBase
{
    private MatchRepository _sut;
    private Fixture _fixture;

    public MatchRepositoryTests() : base() { }

    [TestInitialize]
    public void Initialize()
    {
        _sut = new(Mock.Of<ILogger<MatchRepository>>(), DbContext);
        _fixture = new();
    }

    [TestMethod]
    public async Task AddAsync_SaveChangesIsFalse_ShouldNotSaveMatch()
    {
        //Arrange
        var @event = _fixture.Create<MatchStarted>();

        //Act
        await _sut.AddAsync(@event, false);

        //Assert
        DbContext.Matches.Any().ShouldBeFalse();
    }

    [TestMethod]
    public async Task AddAsync_SaveChangesIsTrue_ShouldSaveMatch()
    {
        //Arrange
        var @event = _fixture.Create<MatchStarted>();

        //Act
        await _sut.AddAsync(@event);

        //Assert
        DbContext.Matches.Any().ShouldBeTrue();
    }

    [TestMethod]
    public async Task  GetAsync_KnownAggregateId_ShouldReturnCorrectMatch()
    {
        //Arrange
        var @event = _fixture.Create<MatchStarted>();

        //Act
        var match = await _sut.AddAsync(@event);
        var result = await _sut.GetAsync(match.AggregateId);

        //Assert
        result?.ShouldNotBeNull();
        result?.ShouldBeEquivalentTo(match);
    }

    [TestMethod]
    public async Task  GetAsync_UnknownAggregateId_ShouldThrowApplicationException()
    {
        //Arrange
        var @event = _fixture.Create<MatchStarted>();

        //Act
        var match = await _sut.AddAsync(@event);
        var randomId = Guid.NewGuid();
        var result = Should.Throw<ApplicationException>(_sut.GetAsync(randomId));

        //Assert
        result.Message.ShouldContain(randomId.ToString());
    }
}