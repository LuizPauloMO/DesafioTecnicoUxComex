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
        private readonly IUxComexServiceDB<Product> ctxProduct;

        public ProductService(IConfiguration configuration)
        {
            this.ctxProduct = new ProductDb(configuration);
        }
        public List<Product>? ListAll()
        {
            return ctxProduct.List();
        }

        public List<Product>? GetByName(String name)
        {
            return ctxProduct.List()?
                             .Where(p => p.Name.Contains(name.Trim()))
                             .ToList();
        }

        public Product? GetById(Int32 Id)
        {
            return ctxProduct.GetById(Id);
        }

        public List<Product>? ListAllInOrderByAsc()
        {
            return ListAll()?.OrderBy(p => p.Name)
                             .ToList();
        }

        public List<Product>? ListAllInOrderByDesc()
        {
            return ListAll()?.OrderByDescending(p => p.Name)
                             .ToList();
        }
        public void Create(Product? product)
        {
            if (product != null)
            {
                try
                {
                    ctxProduct.Add(product);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public async Task Delete(Int32 Id)
        {
            if (Id > 0)
            {
                Product? product_ = ctxProduct.GetById(Id);

                if (product_ != null)
                {
                    try
                    {
                        await ctxProduct.Delete(Id);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }

        public async Task Update(Product? product)
        {
            if (product != null)
            {
                Product? product_ = ctxProduct.GetById(product.Id);

                if (product_ != null)
                {
                    try
                    {
                        await ctxProduct.Update(product);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }
        }

        public Int32 QuantityOfStock(Product product)
        {
            Int32 QuantityOfStock = 0;
            Product? prod = ctxProduct?.GetById(product.Id);

            if (prod != null)
            {
                QuantityOfStock = prod.QuantityOfStock;
            }
            return QuantityOfStock;
        }

        public async Task RemoveProductOfStock(Int32 ProductId, int quantity)
        {
            Product? prod = GetById(ProductId);
            if (prod != null)
            {
                if (prod.QuantityOfStock >= quantity)
                {
                    for (int n = 0; n < quantity; n++)
                    {
                        prod.QuantityOfStock--;

                        try
                        {
                            await Update(prod);
                        }
                        catch (Exception)
                        {
                            throw;
                        }                        
                    }
                }
            }
        }
    }
}
