namespace RestaurantWebApp.Models;

public class Item
{
    public Guid ItemId { get; set; }
    public int CategoryId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImagePath { get; set; } = string.Empty;
    public decimal ItemPrice { get; set; }

    public Category? Category { get; set; }
}
