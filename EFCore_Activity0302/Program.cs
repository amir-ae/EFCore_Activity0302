// See https://aka.ms/new-console-template for more information
using EFCore_DBLibrary;
using InventoryHelpers;
using InventoryModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

Console.WriteLine("Hello, World!");
BuildOptions();
DeleteAllItems();
EnsureItems();
UpdateItems();
ListInventory();

partial class Program
{
    private static IConfigurationRoot? _configuration;
    private static DbContextOptions<InventoryDbContext> _options = new();
    private static readonly Guid _systemUserId = Guid.Parse("2fd28110-93d0-427d-9207-d55dbca680fa");
    private static readonly Guid _loggedInUserId = Guid.NewGuid();

    static void BuildOptions()
    {
        _configuration = ConfigurationBuilderSingleton.ConfigurationRoot;
        DbContextOptionsBuilder<InventoryDbContext> optionsBuilder = new();
        _options = optionsBuilder.UseSqlServer(_configuration.GetConnectionString("InventoryManager")).Options;
    }

    static void EnsureItems()
    {
        EnsureItem("Batman Begins", "You either die the hero or live long enough to see yourself become the villian", "Christian Bale, Katie Holmes");
        EnsureItem("Inception", "You mustn't be afraid to dream a little bigger, darling", "Leonardo DiCaprio, Tom Hardy, Joseph Gordon-Levitt");
        EnsureItem("Remember the Titans", "Left Side, Strong Side", "Denzell Washington, Will Patton");
        EnsureItem("Star Wars: The Empire Strikes Back", "He will join us or die, master", "Harrison Ford, Carrie Fisher, Mark Hamill");
        EnsureItem("Top Gun", "I feel the need, the need for speed!", "Tom Cruise, Anthony Edwards, Val Kilmer");
    }

    private static void EnsureItem(string name, string description, string notes)
    {
        Random random = new Random();

        using var db = new InventoryDbContext(_options);

        var existingItem = db.Items
            .FirstOrDefault(x => x.Name != null && x.Name.ToLower() == name.ToLower());

        if (existingItem is null)
        {
            var item = new Item()
            {
                Name = name,
                CreatedByUserId = _loggedInUserId,
                IsActive = true,
                Quantity = random.Next(1, 10),
                Description = description,
                Notes = notes
            };
            db.Items.Add(item);
            db.SaveChanges();
        }
    }

    private static void ListInventory()
    {
        using var db = new InventoryDbContext(_options);

        var items = db.Items.OrderBy(x => x.Name).ToList();
        items.ForEach(x => Console.WriteLine($"New Item: {x.Name}"));
    }

    private static void DeleteAllItems()
    {
        using var db = new InventoryDbContext(_options);

        var items = db.Items.ToList();
        db.Items.RemoveRange(items);
        db.SaveChanges();
    }

    private static void UpdateItems()
    {
        using var db = new InventoryDbContext(_options);

        var items = db.Items.ToList();
        foreach (var item in items)
        {
            item.CurrentOrFinalPrice = 9.99M;
        }
        db.Items.UpdateRange(items);
        db.SaveChanges();
    }
}