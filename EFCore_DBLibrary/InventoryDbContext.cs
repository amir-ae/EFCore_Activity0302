using Microsoft.EntityFrameworkCore;

namespace EFCore_DBLibrary
{
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext()
        {

        }

        public InventoryDbContext(DbContextOptions options) : base(options)
        {

        }
    }
}