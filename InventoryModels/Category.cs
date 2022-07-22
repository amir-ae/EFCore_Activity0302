using System.ComponentModel.DataAnnotations;

namespace InventoryModels
{
    public class Category : FullAuditModel
    {
        [Required]
        [StringLength(InventoryModelsConstants.MAX_NAME_LENGTH)]
        public string Name { get; set; } = string.Empty;

        public virtual List<Item> Items { get; set; } = new();

        public virtual CategoryDetail CategoryDetail { get; set; } = null!;
    }
}
