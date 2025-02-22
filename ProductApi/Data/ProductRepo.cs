using Microsoft.AspNetCore.Mvc;
using ProductApi.Models;

namespace ProductApi.Data
{
    public class ProductRepo : IProductRepo
    {

        private readonly ProductContext _context;

        public ProductRepo(ProductContext context)
        {
            _context = context;
        }
        public IEnumerable<Product> GetAllProducts() => _context.Products.ToList();

        public Product GetProductById(int id) => _context.Products.FirstOrDefault(p => p.Id == id)!;

        public IActionResult HealthCheck()
        {
            if (_context.Database.CanConnect())
            {
                return new OkObjectResult("API OK");
            }
            else
            {
                return new BadRequestObjectResult("API not connected");
            }
        }
    }
}
