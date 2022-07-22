using AutoMapper;
using EFCore_DBLibrary;
using InventoryModels.DTOs;

namespace InventoryDatabaseLayer
{
    public class CategoriesService : ICategoriesService
    {
        private readonly ICategoriesRepo _dbRepo;

        public CategoriesService(InventoryDbContext dbContext, IMapper mapper)
        {
            _dbRepo = new CategoriesRepo(dbContext, mapper);
        }

        public List<CategoryDto> ListCategoriesAndDetails()
        {
            return _dbRepo.ListCategoriesAndDetails();
        }
    }
}
