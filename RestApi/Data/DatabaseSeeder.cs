using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RestApi.Entities;
using RestApi.Entities.Enums;

namespace RestApi.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
    {
        // Vérifier si la base de données contient déjà des données
        if (await context.Users.AnyAsync())
        {
            Console.WriteLine("La base de données contient déjà des données. Seeding ignoré.");
            return;
        }

        Console.WriteLine("Début du seeding de la base de données...");

        // 1. Créer les rôles
        if (!await roleManager.RoleExistsAsync(Roles.User))
        {
            await roleManager.CreateAsync(new IdentityRole<int>(Roles.User));
        }

        if (!await roleManager.RoleExistsAsync(Roles.Admin))
        {
            await roleManager.CreateAsync(new IdentityRole<int>(Roles.Admin));
        }

        Console.WriteLine("✓ Rôles créés");

        // 2. Seeder les banques
        var banks = new List<Bank>
        {
            new Bank { Name = "BNP Paribas" },
            new Bank { Name = "Crédit Agricole" },
            new Bank { Name = "Société Générale" },
            new Bank { Name = "LCL" }
        };

        await context.Banks.AddRangeAsync(banks);
        await context.SaveChangesAsync();
        Console.WriteLine("✓ 4 banques créées");

        // 3. Seeder les utilisateurs avec UserManager et assigner les rôles
        var usersData = new[]
        {
            new { FirstName = "Alice", LastName = "Martin", Email = "alice@example.com", BirthDate = new DateTime(1990, 5, 15), Role = Roles.Admin },
            new { FirstName = "Bob", LastName = "Dupont", Email = "bob@example.com", BirthDate = new DateTime(1985, 8, 22), Role = Roles.User },
            new { FirstName = "Charlie", LastName = "Bernard", Email = "charlie@example.com", BirthDate = new DateTime(1995, 3, 10), Role = Roles.User }
        };

        var users = new List<User>();
        foreach (var userData in usersData)
        {
            var user = new User
            {
                UserName = userData.Email,
                Email = userData.Email,
                FirstName = userData.FirstName,
                LastName = userData.LastName,
                BirthDate = userData.BirthDate,
                EmailConfirmed = true // Confirmer l'email pour le seeding
            };

            // UserManager gère le hashing du mot de passe automatiquement
            var result = await userManager.CreateAsync(user, "Password123!");

            if (result.Succeeded)
            {
                // Assigner le rôle à l'utilisateur
                await userManager.AddToRoleAsync(user, userData.Role);
                users.Add(user);
            }
            else
            {
                Console.WriteLine($"Erreur lors de la création de l'utilisateur {user.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }

        Console.WriteLine($"✓ {users.Count} utilisateurs créés avec rôles assignés");

        // 4. Seeder les comptes bancaires
        var random = new Random();
        var accounts = new List<Account>();

        foreach (var user in users)
        {
            // Créer 2-3 comptes par utilisateur
            var accountCount = random.Next(2, 4);
            for (int i = 0; i < accountCount; i++)
            {
                var bank = banks[random.Next(banks.Count)];
                var account = new Account
                {
                    Owner = user,
                    Bank = bank,
                    IBAN = GenerateIBAN(),
                    Balance = random.Next(100, 10000)
                };
                accounts.Add(account);
            }
        }

        await context.Accounts.AddRangeAsync(accounts);
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ {accounts.Count} comptes bancaires créés");

        // 5. Seeder les transactions
        var transactions = new List<Transaction>();
        var transactionKinds = new[] { TransactionKind.Deposit, TransactionKind.Withdraw };

        foreach (var account in accounts)
        {
            // Créer 3-5 transactions par compte
            var transactionCount = random.Next(3, 6);
            for (int i = 0; i < transactionCount; i++)
            {
                var kind = transactionKinds[random.Next(transactionKinds.Length)];
                var amount = random.Next(10, 500);

                var transaction = new Transaction
                {
                    AccountId = account.Id,
                    At = DateTime.Now.AddDays(-random.Next(1, 90)), // Transactions des 90 derniers jours
                    Kind = kind,
                    Amount = amount
                };
                transactions.Add(transaction);
            }
        }

        await context.Transactions.AddRangeAsync(transactions);
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ {transactions.Count} transactions créées");

        Console.WriteLine("✅ Seeding terminé avec succès !");
    }

    private static string GenerateIBAN()
    {
        // Générer un IBAN français fictif (FR + 2 chiffres de contrôle + 23 chiffres)
        var random = new Random();
        var checkDigits = random.Next(10, 100);
        var bankCode = random.Next(10000, 100000);
        var branchCode = random.Next(10000, 100000);
        var accountNumber = random.Next(10000000, 100000000);
        var key = random.Next(10, 100);

        return $"FR{checkDigits}{bankCode}{branchCode}{accountNumber:D11}{key}";
    }
}
