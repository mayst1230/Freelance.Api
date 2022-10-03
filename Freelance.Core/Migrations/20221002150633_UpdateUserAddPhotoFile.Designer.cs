﻿// <auto-generated />
using System;
using Freelance.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Freelance.Core.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20221002150633_UpdateUserAddPhotoFile")]
    partial class UpdateUserAddPhotoFile
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("freelance")
                .HasAnnotation("Npgsql:CollationDefinition:public.case_insensitive", "@colStrength=primary,@colStrength=primary,icu,False")
                .HasAnnotation("ProductVersion", "6.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Freelance.Core.Models.Storage.Feedback", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("integer")
                        .HasColumnName("created_by");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("is_deleted");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("text");

                    b.Property<Guid>("UniqueIdentifier")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("unique_identifier")
                        .HasDefaultValueSql("gen_random_uuid()");

                    b.Property<DateTimeOffset>("Updated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.Property<decimal>("UserRating")
                        .HasColumnType("numeric(1,1)")
                        .HasColumnName("user_rating");

                    b.HasKey("Id")
                        .HasName("pk_feedbacks");

                    b.HasIndex("CreatedBy")
                        .HasDatabaseName("ix_feedbacks_created_by");

                    b.HasIndex("UniqueIdentifier")
                        .IsUnique()
                        .HasDatabaseName("ix_uq_feedback");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_feedbacks_user_id");

                    b.ToTable("feedbacks", "freelance");
                });

            modelBuilder.Entity("Freelance.Core.Models.Storage.File", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created");

                    b.Property<int?>("CreatedBy")
                        .HasColumnType("integer")
                        .HasColumnName("created_by");

                    b.Property<byte[]>("Data")
                        .IsRequired()
                        .HasColumnType("bytea")
                        .HasColumnName("data");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("display_name");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("file_name");

                    b.Property<int>("GroupType")
                        .HasColumnType("integer")
                        .HasColumnName("group_type");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("is_deleted");

                    b.Property<string>("MimeType")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("mime_type");

                    b.Property<long>("Size")
                        .HasColumnType("bigint")
                        .HasColumnName("size");

                    b.Property<Guid>("UniqueIdentifier")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("unique_identifier")
                        .HasDefaultValueSql("gen_random_uuid()");

                    b.HasKey("Id")
                        .HasName("pk_files");

                    b.HasIndex("CreatedBy")
                        .HasDatabaseName("ix_files_created_by");

                    b.HasIndex("UniqueIdentifier")
                        .IsUnique()
                        .HasDatabaseName("ix_uq_file");

                    b.ToTable("files", "freelance");
                });

            modelBuilder.Entity("Freelance.Core.Models.Storage.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTimeOffset>("Canceled")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("canceled");

                    b.Property<int?>("ContractorFileId")
                        .HasColumnType("integer")
                        .HasColumnName("contractor_file_id");

                    b.Property<int?>("ContractorId")
                        .HasColumnType("integer")
                        .HasColumnName("contractor_id");

                    b.Property<int?>("CustomerFileId")
                        .HasColumnType("integer")
                        .HasColumnName("customer_file_id");

                    b.Property<int>("CustomerId")
                        .HasColumnType("integer")
                        .HasColumnName("customer_id");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)")
                        .HasColumnName("description");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("is_deleted");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric(10,2)")
                        .HasColumnName("price");

                    b.Property<DateTimeOffset>("Started")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("started");

                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("title");

                    b.Property<Guid>("UniqueIdentifier")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("unique_identifier")
                        .HasDefaultValueSql("gen_random_uuid()");

                    b.HasKey("Id")
                        .HasName("pk_orders");

                    b.HasIndex("ContractorFileId")
                        .HasDatabaseName("ix_orders_contractor_file_id");

                    b.HasIndex("ContractorId")
                        .HasDatabaseName("ix_orders_contractor_id");

                    b.HasIndex("CustomerFileId")
                        .HasDatabaseName("ix_orders_customer_file_id");

                    b.HasIndex("CustomerId")
                        .HasDatabaseName("ix_orders_customer_id");

                    b.HasIndex("UniqueIdentifier")
                        .IsUnique()
                        .HasDatabaseName("ix_uq_order");

                    b.ToTable("orders", "freelance");
                });

            modelBuilder.Entity("Freelance.Core.Models.Storage.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("email");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("first_name");

                    b.Property<bool>("IsBanned")
                        .HasColumnType("boolean")
                        .HasColumnName("is_banned");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("is_deleted");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("last_name");

                    b.Property<string>("MiddleName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("middle_name");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)")
                        .HasColumnName("password");

                    b.Property<int?>("PhotoFileId")
                        .HasColumnType("integer")
                        .HasColumnName("photo_file_id");

                    b.Property<decimal>("Rating")
                        .HasColumnType("numeric(1,1)")
                        .HasColumnName("rating");

                    b.Property<int>("Role")
                        .HasColumnType("integer")
                        .HasColumnName("role");

                    b.Property<Guid>("UniqueIdentifier")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("unique_identifier")
                        .HasDefaultValueSql("gen_random_uuid()");

                    b.Property<DateTimeOffset>("Updated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("user_name");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasIndex("PhotoFileId")
                        .HasDatabaseName("ix_users_photo_file_id");

                    b.HasIndex("UniqueIdentifier")
                        .IsUnique()
                        .HasDatabaseName("ix_uq_user");

                    b.ToTable("users", "freelance");
                });

            modelBuilder.Entity("Freelance.Core.Models.Storage.UserBalance", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.Property<decimal>("Balance")
                        .HasColumnType("numeric(10,2)")
                        .HasColumnName("balance");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created");

                    b.Property<DateTimeOffset>("Updated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated");

                    b.HasKey("UserId")
                        .HasName("pk_user_balances");

                    b.ToTable("user_balances", "freelance");
                });

            modelBuilder.Entity("Freelance.Core.Models.Storage.UserBalanceLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<decimal>("BalanceAfter")
                        .HasColumnType("numeric(10,2)")
                        .HasColumnName("balance_after");

                    b.Property<decimal>("BalanceBefore")
                        .HasColumnType("numeric(10,2)")
                        .HasColumnName("balance_before");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created");

                    b.Property<decimal>("Credit")
                        .HasColumnType("numeric(10,2)")
                        .HasColumnName("credit");

                    b.Property<decimal>("Debit")
                        .HasColumnType("numeric(10,2)")
                        .HasColumnName("debit");

                    b.Property<int>("Type")
                        .HasColumnType("integer")
                        .HasColumnName("type");

                    b.Property<Guid>("UniqueIdentifier")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("unique_identifier")
                        .HasDefaultValueSql("gen_random_uuid()");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_user_balance_logs");

                    b.HasIndex("UniqueIdentifier")
                        .IsUnique()
                        .HasDatabaseName("ix_uq_user_balance_log");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_user_balance_logs_user_id");

                    b.ToTable("user_balance_logs", "freelance");
                });

            modelBuilder.Entity("Freelance.Core.Models.Storage.Feedback", b =>
                {
                    b.HasOne("Freelance.Core.Models.Storage.User", "CreatedByUser")
                        .WithMany()
                        .HasForeignKey("CreatedBy")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_feedbacks_users_created_by_user_id");

                    b.HasOne("Freelance.Core.Models.Storage.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_feedbacks_users_user_id");

                    b.Navigation("CreatedByUser");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Freelance.Core.Models.Storage.File", b =>
                {
                    b.HasOne("Freelance.Core.Models.Storage.User", null)
                        .WithMany()
                        .HasForeignKey("CreatedBy")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("fk_files_users_user_id");
                });

            modelBuilder.Entity("Freelance.Core.Models.Storage.Order", b =>
                {
                    b.HasOne("Freelance.Core.Models.Storage.File", null)
                        .WithMany()
                        .HasForeignKey("ContractorFileId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("fk_orders_files_file_id");

                    b.HasOne("Freelance.Core.Models.Storage.User", null)
                        .WithMany()
                        .HasForeignKey("ContractorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("fk_orders_users_user_id");

                    b.HasOne("Freelance.Core.Models.Storage.File", null)
                        .WithMany()
                        .HasForeignKey("CustomerFileId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("fk_orders_files_file_id1");

                    b.HasOne("Freelance.Core.Models.Storage.User", null)
                        .WithMany()
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_orders_users_user_id1");
                });

            modelBuilder.Entity("Freelance.Core.Models.Storage.User", b =>
                {
                    b.HasOne("Freelance.Core.Models.Storage.File", "PhotoFile")
                        .WithMany()
                        .HasForeignKey("PhotoFileId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("fk_users_files_photo_file_id");

                    b.Navigation("PhotoFile");
                });

            modelBuilder.Entity("Freelance.Core.Models.Storage.UserBalance", b =>
                {
                    b.HasOne("Freelance.Core.Models.Storage.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_user_balances_users_user_id1");
                });

            modelBuilder.Entity("Freelance.Core.Models.Storage.UserBalanceLog", b =>
                {
                    b.HasOne("Freelance.Core.Models.Storage.UserBalance", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_user_balance_logs_user_balances_user_balance_user_id");
                });
#pragma warning restore 612, 618
        }
    }
}
