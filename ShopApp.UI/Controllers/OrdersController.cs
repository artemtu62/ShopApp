using Microsoft.AspNetCore.Mvc;
using ShopApp.Common.Models;

namespace ShopApp.UI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
    {
        // Заглушка: реальный OrderService появится позже.
        return Ok(Array.Empty<Order>());
    }
}