using System.ComponentModel.DataAnnotations;

namespace InventoryModels
{
    public class Player : FullAuditModel
    {
        [Required]
        [StringLength(InventoryModelsConstants.MAX_PLAYERNAME_LENGTH)]
        public string Name { get; set; } = string.Empty;

        [StringLength(InventoryModelsConstants.MAX_PLAYERDESCRIPTION_LENGTH)]
        public string Description { get; set; } = string.Empty;

        public virtual List<Item> Items { get; set; } = new List<Item>();
    }
}
