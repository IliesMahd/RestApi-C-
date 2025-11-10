using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RestApi.Entities;

namespace RestApi;

public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<Bank> Banks { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuration de l'entity User (propriétés personnalisées)
        // Les propriétés de base (Id, Email, PasswordHash, etc.) sont gérées par IdentityDbContext
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.BirthDate).IsRequired();

            // Relation User -> Accounts (One-to-Many)
            entity.HasMany(u => u.Accounts)
                .WithOne(a => a.Owner)
                .HasForeignKey("OwnerId")
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuration de l'entité Bank
        modelBuilder.Entity<Bank>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Name).IsRequired().HasMaxLength(200);

            // Relation Bank -> Accounts (One-to-Many)
            entity.HasMany(b => b.Accounts)
                .WithOne(a => a.Bank)
                .HasForeignKey("BankId")
                .OnDelete(DeleteBehavior.Restrict);

            // Relation Bank -> Transactions (One-to-Many)
            entity.HasMany(b => b.Transactions)
                .WithOne()
                .HasForeignKey("BankId")
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuration de l'entité Account
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.IBAN).IsRequired().HasMaxLength(34);
            entity.Property(a => a.Balance).HasColumnType("decimal(18,2)");

            // Index unique sur IBAN
            entity.HasIndex(a => a.IBAN).IsUnique();
        });

        // Configuration de l'entité Transaction
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Amount).HasColumnType("decimal(18,2)");
            entity.Property(t => t.At).IsRequired();
            entity.Property(t => t.Kind).IsRequired();

            // Relation Transaction -> Account (Many-to-One)
            entity.HasOne<Account>()
                .WithMany()
                .HasForeignKey(t => t.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuration de l'entité RefreshToken
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(rt => rt.Id);
            entity.Property(rt => rt.Token).IsRequired().HasMaxLength(500);
            entity.Property(rt => rt.ExpiresAt).IsRequired();
            entity.Property(rt => rt.CreatedAt).IsRequired();
            entity.Property(rt => rt.IsRevoked).IsRequired();

            // Index unique sur Token
            entity.HasIndex(rt => rt.Token).IsUnique();

            // Relation RefreshToken -> User (Many-to-One)
            entity.HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}