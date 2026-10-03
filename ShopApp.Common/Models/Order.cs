namespace ShopApp.Common.Models;

public class Order
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = "Новый";
    public decimal Total { get; set; }
    public int ClientId { get; set; }
    public Client? Client { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}