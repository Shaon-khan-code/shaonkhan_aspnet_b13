namespace MaleFashion.Application.DTOs;

public class CartDto
{
    public int Id { get; set; }
    public List<CartItemDto> Items { get; set; } = new();

    public decimal Total => Items.Sum(i => i.LineTotal);
    public int TotalItemCount => Items.Sum(i => i.Quantity);
}