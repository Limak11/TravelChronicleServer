﻿// <auto-generated />
using System;
using Chronicle.Web.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Chronicle.Web.Migrations
{
    [DbContext(typeof(ChronicleContext))]
    [Migration("20190925085431_AddedAuthorRelation")]
    partial class AddedAuthorRelation
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Chronicle.Web.Models.APIKey", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("CreatedTime");

                    b.Property<bool>("IsExpired");

                    b.Property<string>("Key");

                    b.Property<long>("UserId");

                    b.HasKey("Id");

                    b.ToTable("APIKeys");
                });

            modelBuilder.Entity("Chronicle.Web.Models.Author", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("ChronicleId");

                    b.Property<long>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("ChronicleId")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.ToTable("Author");
                });

            modelBuilder.Entity("Chronicle.Web.Models.Chronicle", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("CreatedTime");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Chronicle");
                });

            modelBuilder.Entity("Chronicle.Web.Models.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("BirthDate");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Chronicle.Web.Models.Author", b =>
                {
                    b.HasOne("Chronicle.Web.Models.Chronicle", "Chronicle")
                        .WithOne("Author")
                        .HasForeignKey("Chronicle.Web.Models.Author", "ChronicleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Chronicle.Web.Models.User", "User")
                        .WithMany("OwnedChronicles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}