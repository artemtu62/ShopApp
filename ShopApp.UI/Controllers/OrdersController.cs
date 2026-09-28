using Microsoft.AspNetCore.Mvc;
using ShopApp.BLL.Services;

namespace ShopApp.UI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _service;

    public OrdersController(OrderService service) => _service = service;

    [HttpGet]
    public IActionResult GetAll() => Ok(_service.GetOrders());
}