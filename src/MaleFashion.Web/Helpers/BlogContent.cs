using System.Globalization;

namespace MaleFashion.Web.Helpers;


public sealed record BlogPost(string Image, string Title, int DaysAgo)
{
    public DateTime PublishedOn => DateTime.Today.AddDays(-DaysAgo);

    public string DateLabel => PublishedOn.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture);
}

public static class BlogContent
{

    public static IReadOnlyList<BlogPost> Posts { get; } = new List<BlogPost>
    {
        new("blog-1.jpg", "Weekend Style: Easy Outfits for a Coffee Run", 3),
        new("blog-2.jpg", "How to Wear a Plain Black T-Shirt Five Ways", 9),
        new("blog-3.jpg", "Outdoor Ready: Boots and Sunglasses for Your Next Trip", 16),
        new("blog-4.jpg", "Shirt Fit Guide: Slim, Regular or Relaxed?", 24),
        new("blog-5.jpg", "Grooming Essentials Every Man Should Own", 33),
        new("blog-6.jpg", "Denim or Chinos: Which Trousers Suit You?", 41),
        new("blog-7.jpg", "Beard Care Basics: Oil, Balm and Trimming Tips", 52),
        new("blog-8.jpg", "Capsule Wardrobe: Ten Basics That Never Go Out of Style", 64),
        new("blog-9.jpg", "Loungewear and Nightwear for Cooler Evenings", 78),
    };
}
