using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryModels.DTOs
{
    public class ItemDetailDto : ItemDto
    {
        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public string? Notes { get; set; }

        public string? CategoryName { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreatedDate { get; set; }

        public override string ToString()
        {
            return $"ITEM {Name,-35}] {Description,-50} has category: {CategoryName}";
        }
    }
}
