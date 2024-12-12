using Microsoft.EntityFrameworkCore;
using TestovoeZadanie.Models;

namespace TestovoeZadanie.Data
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options)
            : base(options)
        {
        }

        public required DbSet<Product> Products { get; set; }
    }
}
