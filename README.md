# Chess Game
This is a hobby project to try out new technologies and design principles.

Architecture:
- [Domain Driven Design](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/ddd-oriented-microservice)
- [EventSourcing](https://learn.microsoft.com/en-us/azure/architecture/patterns/event-sourcing)

Technologies:
- [Blazor Server](https://learn.microsoft.com/en-us/aspnet/core/blazor/?view=aspnetcore-7.0#blazor-server)
- [bUnit](https://bunit.dev)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [Sqlite3](https://sqlite.org/cli.html)

## Installation
Follow these steps:
- Make sure to have installed `sqlite3` and that it's available in your command prompt.
- When running the application for the first time, make sure to run `dotnet tool install`.
- After this, run `dotnet build`. During the build a SQL script will be generated.
- Run the following command to create the database (from project root).
    ```
    sqlite3 Chess.db
    .read Chess/SQL/ChessDb.sql
    .exit
    ```
- Run `dotnet watch`

## TO DO
- Draw:
  - Ask for draw by aggrement.
  - Ask for draw by repitition.
  - Draw by insufficient material (automatically).
  - Draw by fifty-move rule.

- UI events
  - End the game
  - Pawn promotion

- Castling
  - Pass through check

## Technical Dept
- Implement [FluentResult](https://github.com/altmann/FluentResults) instead of throwing Exceptions.

Alot more idea's will be added soon..