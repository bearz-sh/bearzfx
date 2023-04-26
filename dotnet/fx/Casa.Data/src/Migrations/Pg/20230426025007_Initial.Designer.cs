﻿// <auto-generated />
using System;
using Bearz.Casa.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Bearz.Casa.Data.Migrations.Pg
{
    [DbContext(typeof(PgCasaDbContext))]
    [Migration("20230426025007_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Bearz.Casa.Data.Models.Environment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("display_name");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_environments");

                    b.ToTable("environments", (string)null);
                });

            modelBuilder.Entity("Bearz.Casa.Data.Models.Secret", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<int>("EnvironmentId")
                        .HasColumnType("integer")
                        .HasColumnName("environment_id");

                    b.Property<DateTime?>("ExpiresAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("expires_at");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)")
                        .HasColumnName("name");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)")
                        .HasColumnName("value");

                    b.HasKey("Id")
                        .HasName("pk_secrets");

                    b.HasIndex("EnvironmentId")
                        .HasDatabaseName("ix_secrets_environment_id");

                    b.ToTable("secrets", (string)null);
                });

            modelBuilder.Entity("Bearz.Casa.Data.Models.Setting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("EnvironmentId")
                        .HasColumnType("integer")
                        .HasColumnName("environment_id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)")
                        .HasColumnName("name");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)")
                        .HasColumnName("value");

                    b.HasKey("Id")
                        .HasName("pk_setting");

                    b.HasIndex("EnvironmentId")
                        .HasDatabaseName("ix_setting_environment_id");

                    b.ToTable("setting", (string)null);
                });

            modelBuilder.Entity("Bearz.Casa.Data.Models.Secret", b =>
                {
                    b.HasOne("Bearz.Casa.Data.Models.Environment", "Environment")
                        .WithMany("Secrets")
                        .HasForeignKey("EnvironmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_secrets_environments_environment_id");

                    b.Navigation("Environment");
                });

            modelBuilder.Entity("Bearz.Casa.Data.Models.Setting", b =>
                {
                    b.HasOne("Bearz.Casa.Data.Models.Environment", "Environment")
                        .WithMany("Settings")
                        .HasForeignKey("EnvironmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_setting_environments_environment_id");

                    b.Navigation("Environment");
                });

            modelBuilder.Entity("Bearz.Casa.Data.Models.Environment", b =>
                {
                    b.Navigation("Secrets");

                    b.Navigation("Settings");
                });
#pragma warning restore 612, 618
        }
    }
}
