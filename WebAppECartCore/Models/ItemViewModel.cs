using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebAppECartCore.Models;

public class ItemViewModel
{
    public Guid ItemId { get; set; }
    public int CategoryId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal ItemPrice { get; set; }
    public string CurrentImagePath { get; set; } = string.Empty;
    public IFormFile? ImagePath { get; set; }
    public IEnumerable<SelectListItem> CategorySelectListItem { get; set; } = new List<SelectListItem>();
}
