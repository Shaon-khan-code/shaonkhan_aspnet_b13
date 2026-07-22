using DevSkill.Shop.Domain.Entities;

namespace DevSkill.Shop.Web.Models
{
    public class Stock
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int LowStockQuantity { get; set; }

        public Product Product { get; set; } = null!;
    }
}
