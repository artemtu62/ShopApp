namespace ShopApp.Common.Models;

/// <summary>Статусы заказа и допустимые переходы между ними
/// (см. OrderValidator.EnsureCanTransition).</summary>
public enum OrderStatus
{
    Новый,
    Оплачен,
    Доставляется,
    Доставлен,
    Отменён,
}