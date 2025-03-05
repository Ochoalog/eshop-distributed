using MassTransit;
using ServiceDefaults.Messaging.Events;

namespace Catalog.Services
{
    public class ProductService(ProductDbContext dbContext, IBus bus)
    {
        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await dbContext.Products.ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await dbContext.Products.FindAsync(id);
        }

        public async Task CreateProductAsync(Product product)
        {
            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product updatedProduct, Product inputProduct)
        {
            if(updatedProduct.Price != inputProduct.Price)
            {
                var integrationEvent = new ProductPriceChangedIntegrationEvent
                {
                    ProductId = updatedProduct.Id,
                    Name = inputProduct.Name,
                    Description = inputProduct.Description,
                    Price = inputProduct.Price,
                    ImageUrl = inputProduct.ImageUrl
                };

                await bus.Publish(integrationEvent);
            }

            updatedProduct.Name = inputProduct.Name;
            updatedProduct.Description = inputProduct.Description;
            updatedProduct.ImageUrl = inputProduct.ImageUrl;
            updatedProduct.Price = inputProduct.Price;

            dbContext.Products.Update(updatedProduct);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(Product deletedProducted)
        {
            dbContext.Products.Remove(deletedProducted);
            await dbContext.SaveChangesAsync();
        }
    }
}
