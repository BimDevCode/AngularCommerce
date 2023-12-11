
using System.Text.Json;
using Core.Entities;

namespace Infrastructure.Data
{
    public class StoreContextSeed
    {
        public static async Task SeedAsync(StoreContext storeContext){
            if(!storeContext.ProductBrands.Any()){
                var brandsData = File.ReadAllText("../Infrastructure/Data/SeedFolder/brands.json");
                var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData) ?? throw new Exception();
                storeContext.ProductBrands.AddRange(brands);
            }
            if(!storeContext.ProductTypes.Any()){
                var typesData = File.ReadAllText("../Infrastructure/Data/SeedFolder/types.json");
                var types = JsonSerializer.Deserialize<List<ProductType>>(typesData) ?? throw new Exception();
                storeContext.ProductTypes.AddRange(types);
            }
            if(!storeContext.Products.Any()){
                var productsData = File.ReadAllText("../Infrastructure/Data/SeedFolder/products.json");
                var products = JsonSerializer.Deserialize<List<Product>>(productsData) ?? throw new Exception();
                storeContext.Products.AddRange(products);
            }

            if(storeContext.ChangeTracker.HasChanges()) await storeContext.SaveChangesAsync();
        }
    }
}