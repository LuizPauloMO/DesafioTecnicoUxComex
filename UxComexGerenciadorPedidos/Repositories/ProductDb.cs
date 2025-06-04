using Dapper;
using Microsoft.Data.SqlClient;
using System.Text;
using UxComexGerenciadorPedidos.Domain.Business;
using UxComexGerenciadorPedidos.Domain.Entities;

namespace UxComexGerenciadorPedidos.Repositories
{
    public class ProductDb : IUxComexServiceDB<Product>
    {
        private readonly String? uxComexDbString;
        private Int32 Id;
        public ProductDb(IConfiguration configuration)
        {
            this.uxComexDbString = configuration.GetConnectionString("uxComexDbString");
        }

        public Int32 GetLastID()
        {
            return this.Id;
        }

        public async Task Add(Product product)
        {
            var SqlParams = new
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                QuantityOfStock = product.QuantityOfStock
            };

            StringBuilder strB = new StringBuilder();
            strB.Append("INSERT INTO Product(Name,Description,Price,QuantityOfStock)").AppendLine();
            strB.Append("VALUES(@Name,@Description,@Price,@QuantityOfStock)");


            using (SqlConnection con = new SqlConnection(uxComexDbString))
            {
                con.Open();
                SqlTransaction sqlTransaction = con.BeginTransaction("uxComexTransactionAddProduct");

                try
                {
                    await con.ExecuteAsync(strB.ToString(), SqlParams,sqlTransaction);
                    sqlTransaction.Commit();
                }
                catch (Exception)
                {
                    sqlTransaction.Rollback();
                    throw;
                }
              
                sqlTransaction.Dispose();
                Id = await con.ExecuteScalarAsync<int>("SELECT MAX(Id) FROM [Product]");
                con.Close();
            }
        }

        public async Task Update(Product product)
        {
            var SqlParams = new
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                QuantityOfStock = product.QuantityOfStock
            };

            StringBuilder strB = new StringBuilder();
            strB.Append("UPDATE Product").AppendLine();
            strB.Append("SET Name=@Name,Description=@Description,Price=@Price,Quantity_Of_Stock=@Quantity_Of_Stock").AppendLine();
            strB.Append("WHERE Id=@Id");

            using (SqlConnection con = new SqlConnection(uxComexDbString))
            {
                con.Open();
                SqlTransaction sqlTransaction = con.BeginTransaction("uxComexTransactionAddProduct");

                try
                {
                    await con.ExecuteAsync(strB.ToString(), SqlParams, sqlTransaction);
                    sqlTransaction.Commit();
                }
                catch (Exception)
                {
                    sqlTransaction.Rollback();
                    throw;
                }

                sqlTransaction.Dispose();
                con.Close();
            }
        }

        public async Task Delete(Int32 Id_)
        {
            var SqlParam = new { Id = Id_ };

            StringBuilder strB = new StringBuilder();
            strB.Append("DELETE Product WHERE Id=@Id");

            using (SqlConnection con = new SqlConnection(uxComexDbString))
            {
                con.Open();
                SqlTransaction sqlTransaction = con.BeginTransaction("uxComexTransactionDeleteProduct");

                try
                {
                    await con.ExecuteAsync(strB.ToString(), SqlParam, sqlTransaction);
                    sqlTransaction.Commit();
                }
                catch (Exception)
                {
                    sqlTransaction.Rollback();
                    throw;
                }

                sqlTransaction.Dispose();
                con.Close();
            }
        }

        public List<Product> ReadAll()
        {
            List<Product>? products = null;
            using (SqlConnection con = new SqlConnection(uxComexDbString))
            {
                con.Open();
                products = con.Query<Product>("SELECT * FROM Product").ToList();
                con.Close();
            }
            return products;
        }

        public Product? ReadById(Int32 Id_)
        {
            var SqlParam = new { Id = Id_ };

            Product? product = null;

            using (SqlConnection con = new SqlConnection(uxComexDbString))
            {
                con.Open();
                product = con.Query<Product>("SELECT * FROM Product WHERE Id=@Id", SqlParam).FirstOrDefault();
                con.Close();
            }

            return product;
        }
    }
}
