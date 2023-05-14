﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chess.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initialsetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    AggregateId = table.Column<Guid>(type: "TEXT", nullable: false),
                    BlackPlayerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    WhitePlayerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    StartTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Options = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.AggregateId);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AggregateId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Version = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Data = table.Column<string>(type: "TEXT", nullable: false),
                    FK_MatchEvent_AggregateId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Events_Matches_FK_MatchEvent_AggregateId",
                        column: x => x.FK_MatchEvent_AggregateId,
                        principalTable: "Matches",
                        principalColumn: "AggregateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_FK_MatchEvent_AggregateId",
                table: "Events",
                column: "FK_MatchEvent_AggregateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Matches");
        }
    }
}
