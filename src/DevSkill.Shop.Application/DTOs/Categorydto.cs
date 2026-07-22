using System.ComponentModel.DataAnnotations;

namespace DevSkill.Shop.Application.DTOs
{
    public class CategoryCreateDto
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        [MaxLength(500)]
        public string? ImageName { get; set; }
    }

    public class CategoryUpdateDto : CategoryCreateDto
    {
        public Guid Id { get; set; }
    }

    public class CategoryListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? ImageName { get; set; }
        public int ProductCount { get; set; }
    }
}