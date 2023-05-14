﻿// <auto-generated />
using System;
using Chess.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Chess.Infrastructure.Migrations
{
    [DbContext(typeof(MatchDbContext))]
    partial class MatchDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.5");

            modelBuilder.Entity("Chess.Infrastructure.Entity.Match", b =>
                {
                    b.Property<Guid>("AggregateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("BlackPlayerId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Options")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("WhitePlayerId")
                        .HasColumnType("TEXT");

                    b.HasKey("AggregateId");

                    b.ToTable("Matches");
                });

            modelBuilder.Entity("Chess.Infrastructure.Entity.MatchEvent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("AggregateId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("FK_MatchEvent_AggregateId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Version")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("FK_MatchEvent_AggregateId");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("Chess.Infrastructure.Entity.MatchEvent", b =>
                {
                    b.HasOne("Chess.Infrastructure.Entity.Match", "Match")
                        .WithMany("Events")
                        .HasForeignKey("FK_MatchEvent_AggregateId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Match");
                });

            modelBuilder.Entity("Chess.Infrastructure.Entity.Match", b =>
                {
                    b.Navigation("Events");
                });
#pragma warning restore 612, 618
        }
    }
}
