namespace DevSkill.Shop.Application.DTOs
{
    public class PagedResult<T>
    {
        public IList<T> Data { get; set; } = new List<T>();
        public int Total { get; set; }
        public int TotalDisplay { get; set; }
    }
}

