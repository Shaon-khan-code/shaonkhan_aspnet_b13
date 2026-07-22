namespace DevSkill.Shop.Web.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? ImageName { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
