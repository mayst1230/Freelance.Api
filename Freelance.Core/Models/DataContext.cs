using Freelance.Core.Models.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;

#nullable disable

namespace Freelance.Core.Models;

/// <summary>
/// Контекст данных.
/// </summary>
public class DataContext : DbContext
{
    private readonly IConfiguration _configuration;

    public DataContext(DbContextOptions options, IConfiguration configuration) : base(options)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Пользователи.
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// Счета пользователей.
    /// </summary>
    public DbSet<UserBalance> UserBalances { get; set; }

    /// <summary>
    /// Операции со счетами пользователей.
    /// </summary>
    public DbSet<UserBalanceLog> UserBalanceLogs { get; set; }

    /// <summary>
    /// Заказы услуг.
    /// </summary>
    public DbSet<Order> Orders { get; set; }

    /// <summary>
    /// Отзывы.
    /// </summary>
    public DbSet<Feedback> Feedbacks { get; set; }

    /// <summary>
    /// Хранилище файлов.
    /// </summary>
    public DbSet<Storage.File> Files { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSnakeCaseNamingConvention();
        optionsBuilder.ReplaceService<IHistoryRepository, CamelCaseHistoryContext>();
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString("DB"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("freelance");
        modelBuilder.HasCollation("public.case_insensitive", locale: "@colStrength=primary", provider: "icu", deterministic: false);

        modelBuilder.Entity<User>(e =>
        {
            e.Property(i => i.UniqueIdentifier).HasDefaultValueSql("gen_random_uuid()");
            e.HasIndex(i => new { i.UniqueIdentifier }).IsUnique().HasDatabaseName("ix_uq_user");
            e.HasOne<Storage.File>().WithMany().HasForeignKey(i => i.PhotoFileId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Storage.File>(e =>
        {
            e.Property(i => i.UniqueIdentifier).HasDefaultValueSql("gen_random_uuid()");
            e.HasIndex(i => i.UniqueIdentifier).IsUnique().HasDatabaseName("ix_uq_file");
            e.HasOne<User>().WithMany().HasForeignKey(i => i.CreatedBy).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Order>(e =>
        {
            e.Property(i => i.UniqueIdentifier).HasDefaultValueSql("gen_random_uuid()");
            e.HasIndex(i => i.UniqueIdentifier).IsUnique().HasDatabaseName("ix_uq_order");
            e.HasOne<User>().WithMany().HasForeignKey(i => i.ContractorId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<User>().WithMany().HasForeignKey(i => i.CustomerId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<Storage.File>().WithMany().HasForeignKey(i => i.ContractorFileId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<Storage.File>().WithMany().HasForeignKey(i => i.CustomerFileId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<UserBalance>(e =>
        {
            e.HasOne<User>().WithMany().HasForeignKey(i => i.UserId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<UserBalanceLog>(e =>
        {
            e.Property(i => i.UniqueIdentifier).HasDefaultValueSql("gen_random_uuid()");
            e.HasIndex(i => i.UniqueIdentifier).IsUnique().HasDatabaseName("ix_uq_user_balance_log");
            e.HasOne<UserBalance>().WithMany().HasForeignKey(i => i.UserId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Feedback>(e =>
        {
            e.Property(i => i.UniqueIdentifier).HasDefaultValueSql("gen_random_uuid()");
            e.HasIndex(i => i.UniqueIdentifier).IsUnique().HasDatabaseName("ix_uq_feedback");
            e.HasOne<User>().WithMany().HasForeignKey(i => i.UserId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<User>().WithMany().HasForeignKey(i => i.CreatedBy).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
