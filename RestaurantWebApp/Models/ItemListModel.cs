namespace RestaurantWebApp.Models;

public class ItemListModel
{
    public Guid ItemId { get; set; }
    public string ImagePath { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string ItemCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal ItemPrice { get; set; }
    public string ItemCategory { get; set; } = string.Empty;
}
