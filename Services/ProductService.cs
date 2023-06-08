using PriseChecker.Data;
using PriseChecker.Models;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PriseChecker.Services
{
    public class ProductService
    {
        private readonly DataContext _context;
        private readonly string _ebayApiBaseUrl = "https://api.ebay.com/";

        public ProductService(DataContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetProductPriceFromEbay(string productName)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(_ebayApiBaseUrl);

                // Встановити необхідні заголовки, включаючи ваш App ID (Client ID)
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer {-PriceChe-SBX-0393a2ed6-dffa15a5}");
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                // Виконати запит до eBay API для отримання ціни товару за його назвою
                var response = await httpClient.GetAsync($"browse/v1/item_summary/search?q={productName}&limit=1");
                response.EnsureSuccessStatusCode();

                // Отримати ціну товару з відповіді і повернути її
                var content = await response.Content.ReadAsStringAsync();
                var price = ParsePriceFromResponse(content);

                return price;
            }

            // Повернути значення за замовчуванням, якщо не вдалося отримати ціну
            return 0;
        }

        private decimal ParsePriceFromResponse(string responseContent)
        {
            // Отримати ціну товару з відповіді (припустимо, що ціна знаходиться в форматі JSON)
            // Використовуйте вашу реалізацію для розбору JSON, наприклад, Newtonsoft.Json
            // Приклад розбору JSON:
            // var json = JObject.Parse(responseContent);
            // var price = json["items"][0]["price"]["value"].ToObject<decimal>();
            // return price;

            // В якості прикладу повернемо 0
            return 0;
        }

        public async Task<Product> AddProduct(string productName)
        {
            // Створити новий об'єкт Product
            var product = new Product { Name = productName };

            // Додати продукт до контексту бази даних
            _context.Products.Add(product);

            // Зберегти зміни до бази даних
            await _context.SaveChangesAsync();

            // Повернути створений продукт
            return product;
        }

        public async Task<Product> GetProduct(int productId)
        {
            // Знайти продукт за його ідентифікатором
            var product = await _context.Products.FindAsync(productId);

            // Повернути продукт
            return product;
        }

        public async Task<Product> UpdateProduct(int productId, string newName)
        {
            // Знайти продукт за його ідентифікатором
            var product = await _context.Products.FindAsync(productId);

            if (product != null)
            {
                // Оновити назву продукту
                product.Name = newName;

                // Зберегти зміни до бази даних
                await _context.SaveChangesAsync();
            }

            // Повернути оновлений продукт
            return product;
        }

        public async Task<bool> DeleteProduct(int productId)
        {
            // Знайти продукт за його ідентифікатором
            var product = await _context.Products.FindAsync(productId);

            if (product != null)
            {
                // Видалити продукт з контексту бази даних
                _context.Products.Remove(product);

                // Зберегти зміни до бази даних
                await _context.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public IQueryable<Product> GetProducts()
        {
            // Отримати всі продукти з бази даних
            var products = _context.Products;

            // Повернути запит, який може бути додатково опрацьований або фільтрований
            return products;
        }
    }
}
