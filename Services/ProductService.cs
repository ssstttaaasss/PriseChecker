using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

                // Set the necessary headers, including your App ID (Client ID)
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer -PriceChe-SBX-0393a2ed6-dffa15a5");
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                // Make a request to the eBay API to get the product price by its name
                var response = await httpClient.GetAsync($"browse/v1/item_summary/search?q={productName}&limit=1");
                response.EnsureSuccessStatusCode();

                // Get the product price from the response and return it
                var content = await response.Content.ReadAsStringAsync();
                var price = ParsePriceFromResponse(content);

                return price ?? 0; // Return 0 if price is null
            }
        }

        private decimal? ParsePriceFromResponse(string responseContent)
        {
            var json = JObject.Parse(responseContent);
            var priceToken = json["items"]?.FirstOrDefault()?["price"]?["value"];

            if (priceToken != null && decimal.TryParse(priceToken.ToString(), out decimal price))
            {
                return price;
            }

            return null;
        }

        public async Task<Product> AddProduct(string productName)
        {
            // Create a new Product object
            var product = new Product { Name = productName };

            // Add the product to the database context
            _context.Products.Add(product);

            // Save the changes to the database
            await _context.SaveChangesAsync();

            // Return the created product
            return product;
        }

        public async Task<Product> GetProduct(int productId)
        {
            // Find the product by its identifier
            var product = await _context.Products.FindAsync(productId);

            // Return the product
            return product;
        }

        public async Task<Product> UpdateProduct(int productId, string newName)
        {
            // Find the product by its identifier
            var product = await _context.Products.FindAsync(productId);

            if (product != null)
            {
                // Update the product's name
                product.Name = newName;

                // Save the changes to the database
                await _context.SaveChangesAsync();
            }

            // Return the updated product
            return product;
        }

        public async Task<bool> DeleteProduct(int productId)
        {
            // Find the product by its identifier
            var product = await _context.Products.FindAsync(productId);

            if (product != null)
            {
                // Remove the product from the database context
                _context.Products.Remove(product);

                // Save the changes to the database
                await _context.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public IQueryable<Product> GetProducts()
        {
            // Get all products from the database
            var products = _context.Products;

            // Return the query that can be further processed or filtered
            return products;
        }
    }
}
