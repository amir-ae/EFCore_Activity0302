// See https://aka.ms/new-console-template for more information
using EFCore_Activity0301;
using EFCore_DBLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

Console.WriteLine("Hello, World!");
BuildOptions();

partial class Program
{
    private static IConfigurationRoot? _configuration;
    private static DbContextOptionsBuilder<InventoryDbContext> _optionsBuilder = new();

    static void BuildOptions()
    {
        _configuration = ConfigurationBuilderSingleton.ConfigurationRoot;
        _optionsBuilder.UseSqlServer(_configuration.GetConnectionString("InventoryManager"));
    }
}