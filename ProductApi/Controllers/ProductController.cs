using Microsoft.AspNetCore.Mvc;
using ProductApi.Data;
using ProductApi.Models;

namespace ProductApi.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepo _repo;

        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductRepo repo, ILogger<ProductController> logger)
        {
            _repo = repo;
            _logger = logger;
        }


        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetAllProducts()
        {
            _logger.LogInformation("Getting all products");
            var products = _repo.GetAllProducts();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public ActionResult<Product> GetProductById(int id)
        {
            var product = _repo.GetProductById(id);
            if (product != null)
            {
                return Ok(product);
            }
            return NotFound();
        }

        [HttpGet("Health")]
        public IActionResult HealthCheck()
        {
            return _repo.HealthCheck();
        }
    }
}
