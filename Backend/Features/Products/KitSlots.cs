namespace Backend.Features.Products;

public static class KitSlots
{
    private static readonly Dictionary<string, string[]> Categories = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Backpack"] = ["Backpacking Packs", "Technical Daypacks", "Men's Technical Daypacks"],
        ["Shelter"] = ["3-Season Backpacking Tents", "4-Season Tents", "Backpacking Tents"],
        ["Sleeping Bag"] = ["Down Sleeping Bags", "Synthetic Sleeping Bags", "Backpacking Sleeping Bags"],
        ["Sleeping Pad"] = ["Sleeping Pads", "Air Pads", "Self-inflating Pads", "Closed-cell Foam Pads"],
        ["Pillow"] = ["Camping Pillows", "Camp Pillows"],
    };

    public static string[]? CategoriesFor(string slot) => Categories.GetValueOrDefault(slot);
}
