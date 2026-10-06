using ShopApp.Common.Models;
 
namespace ShopApp.BLL;
 
public class OrderValidator
{
    private readonly IClientExistenceChecker _clientChecker;
 
    public OrderValidator(IClientExistenceChecker clientChecker)
    {
        _clientChecker = clientChecker ?? throw new ArgumentNullException(nameof(clientChecker));
    }
 
    public List<string> Validate(Order order)
    {
        if (order is null)
        {
            throw new ArgumentNullException(nameof(order));
        }
 
        var errors = new List<string>();
 
        if (!_clientChecker.Exists(order.ClientId))
        {
            errors.Add($"Клиент с Id={order.ClientId} не найден.");
        }
 
        if (order.Items is null || order.Items.Count == 0)
        {
            errors.Add("Заказ должен содержать хотя бы одну позицию.");
        }
        else
        {
            foreach (OrderItem item in order.Items)
            {
                if (item.Quantity <= 0)
                {
                    errors.Add($"Товар {item.ProductId}: количество должно быть положительным.");
                }
 
                if (item.Price < 0)
                {
                    errors.Add($"Товар {item.ProductId}: цена не может быть отрицательной.");
                }
            }
        }
 
        return errors;
    }
 
    public void EnsureCanTransition(OrderStatus current, OrderStatus next)
    {
        bool allowed = current switch
        {
            OrderStatus.Новый => next is OrderStatus.Оплачен or OrderStatus.Отменён,
            OrderStatus.Оплачен => next is OrderStatus.Доставляется or OrderStatus.Отменён,
            OrderStatus.Доставляется => next is OrderStatus.Доставлен,
            OrderStatus.Доставлен => false,
            OrderStatus.Отменён => false,
            _ => throw new ArgumentOutOfRangeException(nameof(current)),
        };
 
        if (!allowed)
        {
            throw new InvalidOperationException($"Переход статуса {current} → {next} недопустим.");
        }
    }
}
