using EFCore_DBLibrary;
using InventoryModels;

namespace InventoryDataMigrator
{
    public class BuildCategories
    {
        private readonly InventoryDbContext _context;
        private readonly Guid SEED_USER_ID = Guid.Parse("31412031-7859-429c-ab21-c2e3e8d98042");
        
        public BuildCategories(InventoryDbContext context)
        {
            _context = context;
        }

        public void ExecuteSeed()
        {
            if (_context.Categories.Count() == 0)
            {
                _context.Categories.AddRange(
                    new Category()
                    {
                        IsActive = true,
                        IsDeleted = false,
                        Name = "Movies",
                        CategoryDetail = new CategoryDetail() { ColorValue = "#0000FF", ColorName = "Blue" },
                        CreatedByUserId = SEED_USER_ID
                    },
                    new Category()
                    {
                        IsActive = true,
                        IsDeleted = false,
                        Name = "Books",
                        CategoryDetail = new CategoryDetail() { ColorValue = "#FF0000", ColorName = "Red" },
                        CreatedByUserId = SEED_USER_ID
                    },
                    new Category()
                    {
                        IsActive = true,
                        IsDeleted = false,
                        Name = "Games",
                        CategoryDetail = new CategoryDetail() { ColorValue = "#008000", ColorName = "Green" },
                        CreatedByUserId = SEED_USER_ID
                    }
                );
                _context.SaveChanges();
            }
        }
    }
}
