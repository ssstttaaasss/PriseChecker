using PriseChecker.Data;

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
            // Виконати запит до eBay API для отримання ціни товару
            // Використовуйте вашу реалізацію HTTP клієнта для взаємодії з eBay API
            // Наприклад, використовуючи HttpClient:

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(_ebayApiBaseUrl);

                // Встановити необхідні заголовки, включаючи ваш App ID (Client ID)

                // Виконати запит до eBay API для отримання ціни товару за його назвою

                // Отримати ціну товару з відповіді і повернути її

                // Приклад коду, який виконує запит GET до eBay API:
                // var response = await httpClient.GetAsync($"browse/v1/item_summary/search?q={productName}&limit=1");
                // var content = await response.Content.ReadAsStringAsync();
                // var json = JObject.Parse(content);
                // var price = json["items"][0]["price"]["value"].ToObject<decimal>();
                // return price;
            }

            // Повернути значення за замовчуванням, якщо не вдалося отримати ціну
            return 0;
        }

        // Додаткові методи для роботи з товарами, такі як отримання списку товарів, додавання, оновлення і видалення товарів
        // можна додати за потребою
    }
}