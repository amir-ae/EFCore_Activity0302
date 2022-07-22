using InventoryModels.DTOs;

namespace InventoryDatabaseLayer
{
    public interface ICategoriesService
    {
        List<CategoryDto> ListCategoriesAndDetails();
    }
}
