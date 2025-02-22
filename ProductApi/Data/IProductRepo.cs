using Microsoft.AspNetCore.Mvc;
using ProductApi.Models;

namespace ProductApi.Data
{
    public interface IProductRepo
    {
        public IEnumerable<Product> GetAllProducts();

        public Product GetProductById(int id);

        IActionResult HealthCheck();

    }
}
