using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Runtime.InteropServices;
using UxComexGerenciadorPedidos.Domain.Entities;
using UxComexGerenciadorPedidos.Infraestructure;
using UxComexGerenciadorPedidos.Repositories;

namespace UxComexGerenciadorPedidos.Domain.Business
{
    public class ProductService
    {
        private IUxComexServiceDB<Product> productDb;

        public ProductService(IConfiguration configuration)
        {
            this.productDb = new ProductDb(configuration);
        }
        public List<Product>? ListAll()
        {
            return productDb.ReadAll();
        }

        public List<Product>? GetByName(String name)
        {
            return productDb.ReadAll()?.Where(p => p.Name == name).ToList();
        }

        public Product? GetById(Int32 Id)
        {
            return productDb.ReadById(Id);
        }

        public List<Product>? ListAllInOrderByAsc()
        {
            return ListAll()?.OrderBy(p => p.Name).ToList();
        }

        public List<Product>? ListAllInOrderByDesc()
        {
            return ListAll()?.OrderByDescending(p => p.Name).ToList();
        }
        public async Task Create(Product? product)
        {
            if (product != null)
            {
                await productDb.Add(product);
            }
        }

        public async Task Delete(Int32 Id)
        {
            if (Id > 0)
            {
                Product? product_ = productDb.ReadById(Id);

                if (product_ != null)
                {
                    await productDb.Delete(Id);
                }
            }
        }

        public async Task Update(Product? product)
        {
            if (product != null)
            {
                Product? product_ = productDb.ReadById(product.Id);

                if (product_ != null)
                {
                    await productDb.Update(product);
                }
            }
        }

        public Int32 QuantityOfStock(Product product)
        {
            return productDb?.ReadById(product.Id)?.QuantityOfStock ?? 0;            
        }
    }
}
