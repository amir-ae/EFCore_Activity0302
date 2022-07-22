namespace InventoryModels.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }

        public string Category { get; set; } = string.Empty;

        public CategoryDetailDto CategoryDetail { get; set; } = new();
    }
}