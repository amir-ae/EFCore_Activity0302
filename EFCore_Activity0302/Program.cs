// See https://aka.ms/new-console-template for more information
using EFCore_DBLibrary;
using InventoryHelpers;
using InventoryModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

Console.WriteLine("Hello, World!");
BuildOptions();
EnsureItems();
ListInventory();

partial class Program
{
    private static IConfigurationRoot? _configuration;
    private static DbContextOptionsBuilder<InventoryDbContext> _optionsBuilder = new();

    static void BuildOptions()
    {
        _configuration = ConfigurationBuilderSingleton.ConfigurationRoot;
        _optionsBuilder.UseSqlServer(_configuration.GetConnectionString("InventoryManager"));
    }

    static void EnsureItems()
    {
        EnsureItem("Batman Begins");
        EnsureItem("Inception");
        EnsureItem("Remember the Titans");
        EnsureItem("Star Wars: The Empire Strikes Back");
        EnsureItem("Top Gun");
    }

    private static void EnsureItem(string name)
    {
        using var db = new InventoryDbContext(_optionsBuilder.Options);

        var existingItem = db.Items
            .FirstOrDefault(x => x.Name.ToLower() == name.ToLower());

        if (existingItem is null)
        {
            var item = new Item() { Name = name };
            db.Items.Add(item);
            db.SaveChanges();
        }
    }

    private static void ListInventory()
    {
        using var db = new InventoryDbContext(_optionsBuilder.Options);

        var items = db.Items.OrderBy(x => x.Name).ToList();
        items.ForEach(x => Console.WriteLine($"New Item: {x.Name}"));
    }
}