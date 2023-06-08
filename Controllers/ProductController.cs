using Microsoft.AspNetCore.Mvc;

[Route("api/products")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly ProductService _productService;

    public ProductController(ProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("{productName}")]
    public async Task<IActionResult> GetProductPrice(string productName)
    {
        // Отримати ціну товару за його назвою
        var price = await _productService.GetProductPriceFromEbay(productName);

        if (price == 0)
        {
            return NotFound("Товар не знайдено або не вдалося отримати ціну");
        }

        return Ok(price);
    }

    
}