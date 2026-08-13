namespace RestaurantWebApp.Models;

public class OrderRecordModel
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public int ItemCount { get; set; }
}
