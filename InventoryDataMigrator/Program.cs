// See https://aka.ms/new-console-template for more information
// See https://aka.ms/new-console-template for more information
using EFCore_DBLibrary;
using InventoryDataMigrator;
using InventoryHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

Console.WriteLine("Hello, World!");
BuildOptions();
ApplyMigrations();
ExecuteCustomSeedData();

partial class Program
{
    private static IConfigurationRoot? _configuration;
    private static DbContextOptionsBuilder<InventoryDbContext> _optionsBuilder = new();
    private static readonly Guid _loggedInUserId = Guid.NewGuid();

    private static void BuildOptions()
    {
        _configuration = ConfigurationBuilderSingleton.ConfigurationRoot;
        _optionsBuilder.UseSqlServer(_configuration.GetConnectionString("InventoryManager"));
    }

    private static void ApplyMigrations()
    {
        using var db = new InventoryDbContext(_optionsBuilder.Options);
        db.Database.Migrate();
    }

    private static void ExecuteCustomSeedData()
    {
        using var context = new InventoryDbContext(_optionsBuilder.Options);
        var categories = new BuildCategories(context);
        var items = new BuildItems(context);
        categories.ExecuteSeed();
        items.ExecuteSeed();
    }
}

