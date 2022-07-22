namespace InventoryModels.DTOs
{
    public class GetItemsForListingDto
    {
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; } = string.Empty;

        public string? Notes { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; } = true;

        public string? CategoryName { get; set; } = string.Empty;
    }
}
