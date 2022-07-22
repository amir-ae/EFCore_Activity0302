using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryModels
{
    public class Item : FullAuditModel
    {
        [StringLength(InventoryModelsConstants.MAX_NAME_LENGTH)]
        [Required]
        public string Name { get; set; } = string.Empty;

        [Range(InventoryModelsConstants.MINIMUM_QUANTITY, InventoryModelsConstants.MAXIMUM_QUANTITY)]
        public int Quantity { get; set; }

        [StringLength(InventoryModelsConstants.MAX_DESCRIPTION_LENGTH)]
        public string? Description { get; set; }

        [StringLength(InventoryModelsConstants.MAX_NOTES_LENGTH, MinimumLength = 10)]
        public string? Notes { get; set; }

        [DefaultValue(true)]
        public bool IsOnSale { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? PurchasedDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? SoldDate { get; set; }

        [Column(TypeName = "decimal (9, 2)")]
        [Range(InventoryModelsConstants.MINIMUM_PRICE, InventoryModelsConstants.MAXIMUM_PRICE)]
        public decimal? PurchasePrice { get; set; }

        [Column(TypeName = "decimal (9, 2)")]
        public decimal? CurrentOrFinalPrice { get; set; }

        public int? CategoryId { get; set; }

        public virtual Category? Category { get; set; }

        public virtual List<Player> Players { get; set; } = new();

        public virtual List<ItemGenre> ItemGenres { get; set; } = new();
    }
}