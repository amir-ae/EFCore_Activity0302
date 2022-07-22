// See https://aka.ms/new-console-template for more information
using AutoMapper;
using AutoMapper.QueryableExtensions;
using EFCore_Activity0302;
using EFCore_DBLibrary;
using InventoryDatabaseLayer;
using InventoryHelpers;
using InventoryModels;
using InventoryModels.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("Hello, World!");
BuildOptions();
BuildMapper();
using var db = new InventoryDbContext(_optionsBuilder.Options);
_itemsService = new ItemsService(db, _mapper);
_categoriesService = new CategoriesService(db, _mapper);
ListInventory();
GetItemsForListing();
GetAllActiveItemNames();
GetItemsTotalValues();
GetFullItemDetails();
GetItemsForListingLinq();
ListCategoriesAndColors();

Console.WriteLine("Would you like to create items?");
var createItems = Console.ReadLine()?.StartsWith("y", StringComparison.OrdinalIgnoreCase) ?? false;
if (createItems)
{
    Console.WriteLine("Adding new Item(s)");
    CreateMultipleItems();
    Console.WriteLine("Items added");
    var inventory = _itemsService.GetItems();
    inventory.ForEach(x => Console.WriteLine($"Item: {x}"));
}

Console.WriteLine("Would you like to update items?");
var updateItems = Console.ReadLine()?.StartsWith("y", StringComparison.OrdinalIgnoreCase) ?? false;
if (updateItems)
{
    Console.WriteLine("Updating Item(s)");
    UpdateMultipleItems();
    Console.WriteLine("Items updated");
    var inventory2 = _itemsService.GetItems();
    inventory2.ForEach(x => Console.WriteLine($"Item: {x}"));
}

Console.WriteLine("Would you like to delete items?");
var deleteItems = Console.ReadLine()?.StartsWith("y", StringComparison.OrdinalIgnoreCase) ?? false;
if (deleteItems)
{
    Console.WriteLine("Deleting Item(s)");
    DeleteMultipleItems();
    Console.WriteLine("Items Deleted");
    var inventory3 = _itemsService.GetItems();
    inventory3.ForEach(x => Console.WriteLine($"Item: {x}"));
}

Console.WriteLine("Program Complete");

partial class Program
{
    private static IConfigurationRoot? _configuration;
    private static DbContextOptionsBuilder<InventoryDbContext> _optionsBuilder = new();
    private static readonly Guid _loggedInUserId = Guid.NewGuid();
    private static MapperConfiguration _mapperConfig = null!;
    private static IMapper _mapper = null!;
    private static IServiceProvider? _serviceProvider;
    private static IItemsService? _itemsService;
    private static ICategoriesService? _categoriesService;
    private static List<CategoryDto>? _categories;

    private static void BuildOptions()
    {
        _configuration = ConfigurationBuilderSingleton.ConfigurationRoot;
        _optionsBuilder.UseSqlServer(_configuration.GetConnectionString("InventoryManager"));
    }

    private static void BuildMapper()
    {
        var services = new ServiceCollection();
        services.AddAutoMapper(typeof(InventoryMapper));
        _serviceProvider = services.BuildServiceProvider();

        _mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<InventoryMapper>();
        });
        _mapperConfig.AssertConfigurationIsValid();

        _mapper = _mapperConfig.CreateMapper();
    }


    private static void ListInventory()
    {
        var result = _itemsService!.GetItems();
        result.ForEach(x => Console.WriteLine($"New Item: {x}"));
    }

    private static void ListCategoriesAndColors()
    {
        var results = _categoriesService!.ListCategoriesAndDetails();
        _categories = results;

        foreach (var c in results)
        {
            Console.WriteLine($"Category [{c.Category}] is {c.CategoryDetail.Color}");
        }
    }

    private static void GetItemsForListing()
    {
        var results = _itemsService!.GetItemsForListingFromProcedure();
        foreach (var item in results)
        {
            var output = $"ITEM {item.Name}] {item.Name}";
            if (!string.IsNullOrWhiteSpace(item.CategoryName))
            {
                output = $"{output} has category: {item.CategoryName}";
            }
            Console.WriteLine(output);
        }
    }

    private static void GetItemsForListingLinq()
    {
        var minDateValue = new DateTime(2021, 1, 1);
        var maxDateValue = new DateTime(2024, 1, 1);

        var results = _itemsService!.GetItemDetailsByDateRange(minDateValue, maxDateValue)
            .OrderBy(y => y.CategoryName).ThenBy(z => z.Name);

        /*using var db = new InventoryDbContext(_optionsBuilder.Options);
        var results = db.Items.Select(x => new ItemDetailDto
        {
            Name = x.Name,
            Description = x.Description,
            Notes = x.Notes,
            IsActive = x.IsActive,
            IsDeleted = x.IsDeleted,
            CategoryId = x.CategoryId,
            CategoryName = x.Category!.Name,
            Id = x.Id,
            CreatedDate = x.CreatedDate
        }).Where(x => x.CreatedDate >= minDateValue && x.CreatedDate <= maxDateValue)
            .OrderBy(y => y.CategoryName).ThenBy(z => z.Name)
            .ToList();*/

        foreach (var item in results)
        {
            Console.WriteLine(item);
        }
    }

    private static void GetAllActiveItemNames()
    {
        using var db = new InventoryDbContext(_optionsBuilder.Options);
        var isActiveParm = new SqlParameter("IsActive", 1);
        var result = db.AllItemNames.FromSqlRaw("SELECT dbo.ItemNamesPipeDelimitedString (@IsActive) AllItemNames",
            isActiveParm).FirstOrDefault();
        if (result is null)
        {
            Console.WriteLine($"No active items.");
            return;
        } 
        Console.WriteLine($"All active items: {result.AllItemNames}");
    }

    private static void GetItemsTotalValues()
    {
        var results = _itemsService!.GetItemsTotalValues(true);
        foreach (var item in results)
        {
            Console.WriteLine($"New Item] {item.Id,-10}" + 
                $"|{item.Name,-50}" + 
                $"|{item.Quantity,-4}" + 
                $"|{item.TotalValue,-5}");
        }
    }

    private static void GetFullItemDetails()
    {
        var results = _itemsService!.GetItemsWithGenresAndCategories();
        foreach (var item in results)
        {
            Console.WriteLine($"New Item] {item.Id,-10}" +
                $"|{item.ItemName,-50}" +
                $"|{item.ItemDescription,-4}" +
                $"|{item.PlayerName,-5}" +
                $"|{item.Category,-5}" +
                $"|{item.GenreName,-5}"
            );
        }
    }

    private static void CreateMultipleItems()
    {
        Console.WriteLine("Would you like to create items as a batch?");
        bool batchCreate = Console.ReadLine()?.StartsWith("y", StringComparison.OrdinalIgnoreCase) ?? false;

        var allItems = new List<CreateOrUpdateItemDto>();
        bool createAnother = true;

        while (createAnother == true)
        {
            var newItem = new CreateOrUpdateItemDto();
            Console.WriteLine("Creating a new item.");

            Console.WriteLine("Please enter the name");
            newItem.Name = Console.ReadLine() ?? string.Empty;

            Console.WriteLine("Please enter the description");
            newItem.Description = Console.ReadLine();

            Console.WriteLine("Please enter the notes");
            newItem.Notes = Console.ReadLine();

            Console.WriteLine("Please enter the Category [B]ooks, [M]ovies, [G]ames");
            newItem.CategoryId = GetCategoryId(Console.ReadLine()?.Substring(0, 1).ToUpper() ?? string.Empty);

            if (!batchCreate)
            {
                _itemsService!.UpsertItem(newItem);
            }
            else
            {
                allItems.Add(newItem);
            }

            Console.WriteLine("Would you like to create another item?");
            createAnother = Console.ReadLine()?.StartsWith("y", StringComparison.OrdinalIgnoreCase) ?? false;

            if (batchCreate && !createAnother)
            {
                _itemsService!.UpsertItems(allItems);
            }
        }
    }

    private static int GetCategoryId(string input)
    {
        switch (input)
        {
            case "B":
                return _categories?.FirstOrDefault(x => x.Category.ToLower().Equals("books"))?.Id ?? -1;
            case "M":
                return _categories?.FirstOrDefault(x => x.Category.ToLower().Equals("movies"))?.Id ?? -1;
            case "G":
                return _categories?.FirstOrDefault(x => x.Category.ToLower().Equals("games"))?.Id ?? -1;
            default:
                return -1;
        }
    }

    private static void UpdateMultipleItems()
    {
        Console.WriteLine("Would you like to update items as a batch?");
        bool batchUpdate = Console.ReadLine()?.StartsWith("y", StringComparison.OrdinalIgnoreCase) ?? false;

        var allItems = new List<CreateOrUpdateItemDto>();
        bool updateAnother = true;

        while (updateAnother == true)
        {
            Console.WriteLine("Items");
            Console.WriteLine("Enter the ID number to update");

            Console.WriteLine("*******************************");
            var items = _itemsService!.GetItems();
            items.ForEach(x => Console.WriteLine($"ID: {x.Id} | {x.Name}"));
            Console.WriteLine("*******************************");

            int id = 0;
            if (int.TryParse(Console.ReadLine(), out id))
            {
                var itemMatch = items.FirstOrDefault(x => x.Id == id);
                if (itemMatch is not null)
                {
                    var updItem = _mapper.Map<CreateOrUpdateItemDto>(_mapper.Map<Item>(itemMatch));

                    Console.WriteLine("Enter the new name [leave blank to keep existing]");
                    var newName = Console.ReadLine();
                    updItem.Name = !string.IsNullOrWhiteSpace(newName) ? newName : updItem.Name;

                    Console.WriteLine("Enter the new desc [leave blank to keep existing]");
                    var newDesc = Console.ReadLine();
                    updItem.Description = !string.IsNullOrWhiteSpace(newDesc) ? newDesc : updItem.Description;

                    Console.WriteLine("Enter the new notes [leave blank to keep existing]");
                    var newNotes = Console.ReadLine();
                    updItem.Notes = !string.IsNullOrWhiteSpace(newNotes) ? newNotes : updItem.Notes;

                    Console.WriteLine("Toggle Item Active Status? [y/n]");
                    var toggleActive = Console.ReadLine()?.Substring(0, 1).Equals("y", StringComparison.OrdinalIgnoreCase) ?? false;
                    if (toggleActive)
                    {
                        updItem.IsActive = !updItem.IsActive;
                    }

                    Console.WriteLine("Enter the category - [B]ooks, [M]ovies, [G]ames, or [N]o Change");
                    var userChoice = Console.ReadLine()?.Substring(0, 1).ToUpper() ?? "N";

                    updItem.CategoryId = userChoice.Equals("N", StringComparison.OrdinalIgnoreCase) ? itemMatch.CategoryId : GetCategoryId(userChoice);

                    if (!batchUpdate)
                    {
                        _itemsService.UpsertItem(updItem);
                    }
                    else
                    {
                        allItems.Add(updItem);
                    }
                }
            }

            Console.WriteLine("Would you like to update another?");
            updateAnother = Console.ReadLine()?.StartsWith("y", StringComparison.OrdinalIgnoreCase) ?? false;
            
            if (batchUpdate && !updateAnother)
            {
                _itemsService.UpsertItems(allItems);
            }
        }
    }

    private static void DeleteMultipleItems()
    {
        Console.WriteLine("Would you like to delete items as a batch?");
        bool batchDelete = Console.ReadLine()?.StartsWith("y", StringComparison.OrdinalIgnoreCase) ?? false;

        var allItems = new List<int>();
        bool deleteAnother = true;

        while (deleteAnother == true)
        {
            Console.WriteLine("Items");
            Console.WriteLine("Enter the ID number to delete");

            Console.WriteLine("*******************************");
            var items = _itemsService!.GetItems();
            items.ForEach(x => Console.WriteLine($"ID: {x.Id} | {x.Name}"));
            Console.WriteLine("*******************************");

            if (batchDelete && allItems.Any())
            {
                Console.WriteLine("Items scheduled for delete");
                allItems.ForEach(x => Console.Write($"{x},"));
                Console.WriteLine();
                Console.WriteLine("*******************************");
            }

            int id = 0;
            if (int.TryParse(Console.ReadLine(), out id))
            {
                var itemMatch = items.FirstOrDefault(x => x.Id == id);
                if (itemMatch is not null)
                {
                    if (batchDelete)
                    {
                        if (!allItems.Contains(itemMatch.Id))
                        {
                            allItems.Add(itemMatch.Id);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Are you sure you want to delete the item {itemMatch.Id}-{itemMatch.Name}");
                        if (Console.ReadLine()?.StartsWith("y", StringComparison.OrdinalIgnoreCase) ?? false)
                        {
                            _itemsService.DeleteItem(itemMatch.Id);
                            Console.WriteLine("Item Deleted");
                        }
                    }
                }
            }
            Console.WriteLine("Would you like to delete another item?");
            deleteAnother = Console.ReadLine()?.StartsWith("y", StringComparison.OrdinalIgnoreCase) ?? false;

            if (batchDelete && !deleteAnother)
            {
                Console.WriteLine("Are you sure you want to delete the following items: ");
                allItems.ForEach(x => Console.Write($"{x},"));
                Console.WriteLine();

                if (Console.ReadLine()?.StartsWith("y", StringComparison.OrdinalIgnoreCase) ?? false)
                {
                    _itemsService.DeleteItems(allItems);
                    Console.WriteLine("Items Deleted");
                }
            }
        }
    }

    /*private static void EnsureItems()
    {
        EnsureItem("Batman Begins", "You either die the hero or live long enough to see yourself become the villian", "Christian Bale, Katie Holmes");
        EnsureItem("Inception", "You mustn't be afraid to dream a little bigger, darling", "Leonardo DiCaprio, Tom Hardy, Joseph Gordon-Levitt");
        EnsureItem("Remember the Titans", "Left Side, Strong Side", "Denzell Washington, Will Patton");
        EnsureItem("Star Wars: The Empire Strikes Back", "He will join us or die, master", "Harrison Ford, Carrie Fisher, Mark Hamill");
        EnsureItem("Top Gun", "I feel the need, the need for speed!", "Tom Cruise, Anthony Edwards, Val Kilmer");
    }

    private static void EnsureItem(string name, string description, string notes)
    {
        using var db = new InventoryDbContext(_optionsBuilder.Options);
        Random random = new Random();
        var existingItem = db.Items
            .FirstOrDefault(x => x.Name != null && x.Name.ToLower() == name.ToLower());
        if (existingItem is null)
        {
            var item = new Item()
            {
                Name = name,
                CreatedByUserId = _loggedInUserId,
                IsActive = true,
                Quantity = random.Next(1, 1000),
                Description = description,
                Notes = notes
            };
            db.Items.Add(item);
            db.SaveChanges();
        }
    }

    private static void DeleteAllItems()
    {
        using var db = new InventoryDbContext(_optionsBuilder.Options);
        var items = db.Items.ToList();
        db.Items.RemoveRange(items);
        db.SaveChanges();
    }

    private static void UpdateItems()
    {
        using var db = new InventoryDbContext(_optionsBuilder.Options);
        var items = db.Items.ToList();
        foreach (var item in items)
        {
            item.CurrentOrFinalPrice = 9.99M;
        }
        db.Items.UpdateRange(items);
        db.SaveChanges();
    }*/
}