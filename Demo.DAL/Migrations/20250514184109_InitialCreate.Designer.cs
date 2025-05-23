﻿// <auto-generated />
using System;
using Demo.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Demo.DAL.Migrations
{
    [DbContext(typeof(DemoDbContext))]
    [Migration("20250514184109_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.15")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Demo.DAL.Models.ProjectTask", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<long?>("AssignedWorkerId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Department")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("AssignedWorkerId")
                        .IsUnique();

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("Demo.DAL.Models.Worker", b =>
                {
                    b.Property<long>("TelegramId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("TelegramId"));

                    b.Property<string>("Department")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("TelegramUsername")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("WorkerStatus")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("TelegramId");

                    b.ToTable("Workers");
                });

            modelBuilder.Entity("Demo.DAL.Models.ProjectTask", b =>
                {
                    b.HasOne("Demo.DAL.Models.Worker", "AssignedWorker")
                        .WithOne("AssignedTask")
                        .HasForeignKey("Demo.DAL.Models.ProjectTask", "AssignedWorkerId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("AssignedWorker");
                });

            modelBuilder.Entity("Demo.DAL.Models.Worker", b =>
                {
                    b.Navigation("AssignedTask");
                });
#pragma warning restore 612, 618
        }
    }
}
