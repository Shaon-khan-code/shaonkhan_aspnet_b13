using DevSkill.Shop.Domain.Entities;

namespace DevSkill.Shop.Web.Models
{
    public class ProductImage
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Serial { get; set; }

        public Product Product { get; set; } = null!;
    }
}
