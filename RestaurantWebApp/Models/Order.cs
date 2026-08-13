namespace RestaurantWebApp.Models;

public class Order
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public string OrderNumber { get; set; } = string.Empty;

    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
