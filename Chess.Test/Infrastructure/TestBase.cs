using System;
using Chess.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Chess.Test.Infrastructure;

public abstract class TestBase: IDisposable
{
    private const string connectionString = "Data Source=InMemoryDb;Mode=Memory;Cache=Shared";

    private readonly SqliteConnection _connection;
    private bool _disposed;

    public MatchDbContext DbContext { get; private set;}

    public TestBase()
    {
        _connection = new(connectionString);
        _connection.Open();

        var options = new DbContextOptionsBuilder<MatchDbContext>()
                          .UseSqlite(_connection)
                          .Options;

        DbContext = new MatchDbContext(options);
        DbContext.Database.EnsureCreated();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }
        if (disposing)
        {
            _connection?.Dispose();
        }

        _disposed = true;
    }

    public void Dispose() => Dispose(true);
}