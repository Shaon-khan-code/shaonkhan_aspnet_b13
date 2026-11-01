
using System.Collections;
using System.Globalization;

namespace MaleFashion.Web.Helpers;

public sealed record CartLine(
    int Id,
    int ProductId,
    string Name,
    string Image,
    int Quantity,
    decimal UnitPrice,
    decimal LineTotal);

public static class ModelReader
{
    public static object? Get(this object? source, params string[] names)
    {
        if (source is null)
            return null;

        var type = source.GetType();

        foreach (var name in names)
        {
            var property = type.GetProperty(name);

            if (property is not null &&
                property.GetIndexParameters().Length == 0)
            {
                return property.GetValue(source);
            }
        }

        return null;
    }

    public static string GetText(this object? source, params string[] names)
        => source.Get(names)?.ToString() ?? string.Empty;

    public static int GetInt(
        this object? source,
        int fallback,
        params string[] names)
    {
        var value = source.Get(names);

        if (value is null)
            return fallback;

        try
        {
            return Convert.ToInt32(value, CultureInfo.InvariantCulture);
        }
        catch
        {
            return fallback;
        }
    }

    public static decimal? GetDecimal(
        this object? source,
        params string[] names)
    {
        var value = source.Get(names);

        if (value is null)
            return null;

        try
        {
            return Convert.ToDecimal(value, CultureInfo.InvariantCulture);
        }
        catch
        {
            return null;
        }
    }

    public static IReadOnlyList<object> GetList(
        this object? source,
        params string[] names)
    {
        if (source.Get(names) is IEnumerable sequence and not string)
        {
            return sequence.Cast<object>()
                .Where(item => item is not null)
                .ToList();
        }

        return Array.Empty<object>();
    }

    public static string Money(decimal value)
        => "$" + value.ToString("N2", CultureInfo.InvariantCulture);

    public static string ProductImage(object? raw)
    {
        var value = raw?.ToString()?.Trim();

        if (string.IsNullOrWhiteSpace(value))
            return "/storefront/img/no-image.svg";

        // Keep complete S3, CloudFront, or external image URLs.
        if (Uri.TryCreate(value, UriKind.Absolute, out var uri) &&
            (uri.Scheme == Uri.UriSchemeHttp ||
             uri.Scheme == Uri.UriSchemeHttps))
        {
            return value;
        }

        // Keep an application-root URL.
        if (value.StartsWith('/'))
            return value;

        // Legacy database path: img/product/product-2.jpg
        if (value.StartsWith("img/", StringComparison.OrdinalIgnoreCase))
            return "/storefront/" + value;

        // Legacy path: storefront/img/product/product-2.jpg
        if (value.StartsWith("storefront/", StringComparison.OrdinalIgnoreCase))
            return "/" + value;

        // Filename only: product-2.jpg
        return "/storefront/img/product/" + value;
    }

    public static IReadOnlyList<CartLine> ToCartLines(this object? cart)
    {
        return cart.GetList("Items", "CartItems", "Lines")
            .Select(item =>
            {
                var quantity = item.GetInt(1, "Quantity", "Qty");
                var unitPrice = item.GetDecimal(
                    "UnitPrice", "Price", "ProductPrice") ?? 0m;

                return new CartLine(
                    Id: item.GetInt(0, "Id", "CartItemId"),
                    ProductId: item.GetInt(0, "ProductId"),
                    Name: item.GetText("ProductName", "Name", "Title"),
                    Image: ProductImage(
                        item.Get("ImageUrl", "ImageKey",
                                 "ProductImageUrl", "ImagePath")),
                    Quantity: quantity,
                    UnitPrice: unitPrice,
                    LineTotal: item.GetDecimal(
                        "LineTotal", "TotalPrice", "Total", "SubTotal")
                        ?? unitPrice * quantity);
            })
            .ToList();
    }

    public static (decimal Subtotal, decimal Total) GetCartTotals(
        this object? cart,
        IEnumerable<CartLine> lines)
    {
        var sum = lines.Sum(line => line.LineTotal);
        var subtotal = cart.GetDecimal("SubTotal", "Subtotal") ?? sum;
        var total = cart.GetDecimal(
            "TotalAmount", "GrandTotal", "Total") ?? subtotal;

        return (subtotal, total);
    }

    public static bool IsOnSale(this object? product)
    {
        var price = product.GetDecimal("Price", "UnitPrice", "BasePrice");
        var discount = product.GetDecimal(
            "DiscountPrice", "SalePrice", "DiscountedPrice");

        return price.HasValue &&
               discount.HasValue &&
               discount.Value > 0 &&
               discount.Value < price.Value;
    }
}

