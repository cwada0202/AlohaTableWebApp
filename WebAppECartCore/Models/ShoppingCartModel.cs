namespace WebAppECartCore.Models;

public class ShoppingCartModel
{
    public string ItemId { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }
    public string ImagePath { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
}
