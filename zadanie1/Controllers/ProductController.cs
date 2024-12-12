using Microsoft.AspNetCore.Mvc;
using TestovoeZadanie.Data;
using TestovoeZadanie.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace TestovoeZadanie.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductDbContext _context;
        private readonly ILogger<ProductController> _logger;

        public ProductController(ProductDbContext context, ILogger<ProductController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            try
            {
                var products = await _context.Products.ToListAsync();
                _logger.LogInformation("Успешно получен список всех продуктов.");
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка продуктов");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);

                if (product == null)
                {
                    _logger.LogWarning($"Продукт с ID {id} не найден.");
                    return NotFound($"Продукт с ID {id} не найден.");
                }

                _logger.LogInformation($"Продукт с ID {id} успешно найден.");
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при получении продукта с ID {id}");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            try
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Продукт с ID {product.Id} успешно создан.");

                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании продукта.");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                _logger.LogWarning($"Неверный ID в запросе: ожидаемый {id}, полученный {product.Id}");
                return BadRequest($"Неверный ID в запросе: ожидаемый {id}, полученный {product.Id}");
            }

            try
            {
                _context.Entry(product).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Продукт с ID {id} успешно обновлен.");

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    _logger.LogWarning($"Продукт с ID {id} не найден при обновлении.");
                    return NotFound($"Продукт с ID {id} не найден.");
                }
                else
                {
                    _logger.LogError($"Ошибка конкурентного обновления для продукта с ID {id}.");
                    return StatusCode(500, "Ошибка конкурентного обновления.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при обновлении продукта с ID {id}");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    _logger.LogWarning($"Продукт с ID {id} не найден при попытке удаления.");
                    return NotFound($"Продукт с ID {id} не найден.");
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Продукт с ID {id} успешно удален.");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при удалении продукта с ID {id}");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
