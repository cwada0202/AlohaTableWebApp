namespace WebAppECartCore.Models;

public class ShoppingViewModel
{
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public decimal ItemPrice { get; set; }
    public string ImagePath { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ItemCode { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}
