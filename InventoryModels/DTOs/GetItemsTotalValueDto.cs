using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryModels.DTOs
{
    public class GetItemsTotalValueDto
    {
        public int Id { get; set; }
        
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; } = string.Empty;

        public int Quantity { get; set; }

        [Column(TypeName = "decimal (9, 2)")]
        public decimal? PurchasePrice { get; set; }

        [Column(TypeName = "decimal (9, 2)")]
        public decimal? TotalValue { get; set; }
    }
}
