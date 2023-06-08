using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PriseChecker.Models;

namespace PriseChecker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ProductDbContext _dbContext;

        public ProductController(ProductDbContext dbContext)
        {
            _dbContext = dbContext;

            _httpClient = new HttpClient();
            // Налаштування ключа API eBay
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer PriceChe-SBX-0393a2ed6-dffa15a5");
        }

        [HttpGet("{productName}")]
        public async Task<IActionResult> GetProductPrice(string productName)
        {
            try
            {
                // Пошук ціни товару в базі даних
                var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Name == productName);
                if (product != null)
                {
                    // Якщо ціна товару вже збережена, повертаємо її з бази даних
                    return Ok(new { ProductName = productName, Price = product.Price });
                }

                // Формування запиту до API eBay Browse
                string requestUrl = $"https://api.ebay.com/buy/browse/v1/item_summary/search?q={productName}&limit=1";

                // Виконання запиту та отримання відповіді
                HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);

                // Перевірка статусу відповіді
                if (response.IsSuccessStatusCode)
                {
                    // Отримання JSON-даних з відповіді
                    string json = await response.Content.ReadAsStringAsync();

                    // Десеріалізація JSON в об'єкт
                    var result = JsonConvert.DeserializeObject<dynamic>(json);

                    // Отримання ціни товару з отриманих даних
                    var item = result?.itemSummaries?[0];
                    var price = item?.price?.value;

                    if (price != null)
                    {
                        // Збереження ціни товару до бази даних
                        product = new Product { Name = productName, Price = price };
                        _dbContext.Products.Add(product);
                        await _dbContext.SaveChangesAsync();

                        // Повернення ціни товару у відповідь
                        return Ok(new { ProductName = productName, Price = price });
                    }
                }

                // Якщо запит до eBay API не вдалося виконати або не отримано ціну товару, повертаємо відповідь з помилкою
                return NotFound();
            }
            catch (Exception ex)
            {
                // Обробка винятків
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
