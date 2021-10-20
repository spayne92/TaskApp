﻿// <auto-generated />
using System;
using BaseCoreAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BaseCoreAPI.Migrations
{
    [DbContext(typeof(BaseContext))]
    [Migration("20210619061853_InitialDb")]
    partial class InitialDb
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.7");

            modelBuilder.Entity("BaseCoreAPI.Data.Entities.Room", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Rooms");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Kitchen"
                        });
                });

            modelBuilder.Entity("BaseCoreAPI.Data.Entities.Surface", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<int?>("RoomId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RoomId");

                    b.ToTable("Surface");
                });

            modelBuilder.Entity("BaseCoreAPI.Data.Entities.Task", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("Frequency")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<bool>("Repeating")
                        .HasColumnType("tinyint(1)");

                    b.Property<int?>("SurfaceId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SurfaceId");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("BaseCoreAPI.Data.Entities.Surface", b =>
                {
                    b.HasOne("BaseCoreAPI.Data.Entities.Room", null)
                        .WithMany("Surfaces")
                        .HasForeignKey("RoomId");
                });

            modelBuilder.Entity("BaseCoreAPI.Data.Entities.Task", b =>
                {
                    b.HasOne("BaseCoreAPI.Data.Entities.Surface", null)
                        .WithMany("Tasks")
                        .HasForeignKey("SurfaceId");
                });

            modelBuilder.Entity("BaseCoreAPI.Data.Entities.Room", b =>
                {
                    b.Navigation("Surfaces");
                });

            modelBuilder.Entity("BaseCoreAPI.Data.Entities.Surface", b =>
                {
                    b.Navigation("Tasks");
                });
#pragma warning restore 612, 618
        }
    }
}