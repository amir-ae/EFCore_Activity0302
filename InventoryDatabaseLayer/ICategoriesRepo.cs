using InventoryModels.DTOs;

namespace InventoryDatabaseLayer
{
    public interface ICategoriesRepo
    {
        List<CategoryDto> ListCategoriesAndDetails();
    }
}
