using Dapper;
using Microsoft.Data.SqlClient;
using System.Text;
using UxComexGerenciadorPedidos.Domain.Business;
using UxComexGerenciadorPedidos.Domain.Entities;

namespace UxComexGerenciadorPedidos.Repositories
{
    public class OrderItemDb : IUxComexServiceDB<OrderItem>
    {
        private readonly String? uxComexDbString;
        private Int32 Id;

        public OrderItemDb(IConfiguration configuration)
        {
            this.uxComexDbString = configuration.GetConnectionString("uxComexDbString");
        }

        public Int32 GetLastID()
        {
            return this.Id;
        }
        public void Add(OrderItem orderItem)
        {
            var SqlParams = new
            {
                IdOrder = orderItem.IdOrder,
                IdProduct = orderItem.IdProduct,
                UnityPrice = orderItem.UnityPrice,
                Quantity = orderItem.Quantity
            };


            StringBuilder strB = new StringBuilder();
            strB.AppendLine("INSERT INTO OrderItem(IdOrder,IdProduct,UnityPrice,Quantity)");
            strB.AppendLine("VALUES(@IdOrder,@IdProduct,@UnityPrice,@Quantity)");
            strB.AppendLine("SELECT SCOPE_IDENTITY()");

            using (SqlConnection con = new SqlConnection(uxComexDbString))
            {
                con.Open();
                SqlTransaction sqlTransaction = con.BeginTransaction("uxComexTransactionAddOrderItem");

                try
                {
                    Id=con.ExecuteScalar<int>(strB.ToString(), SqlParams, sqlTransaction);
                    sqlTransaction.Commit();
                }
                catch (Exception)
                {
                    sqlTransaction.Rollback();
                    throw;
                }

                sqlTransaction.Dispose();
                //Id = await con.ExecuteScalarAsync<int>("SELECT SCOPE_IDENTITY()");
                con.Close();
            }
        }

        public async Task Update(OrderItem orderItem)
        {
            var SqlParams = new
            {
                Id= orderItem.Id,
                IdOrder = orderItem.IdOrder,
                IdProduct = orderItem.IdProduct,
                UnityPrice = orderItem.UnityPrice,
                Quantity = orderItem.Quantity
            };

            StringBuilder strB = new StringBuilder();
            strB.Append("UPDATE OrderItem").AppendLine();
            strB.Append("SET IdOrder=@IdOrder,IdProduct=@IdProduct,UnityPrice=@UnityPrice,Quantity=@Quantity").AppendLine();
            strB.Append("WHERE Id=@Id");


            using (SqlConnection con = new SqlConnection(uxComexDbString))
            {
                con.Open();
                SqlTransaction sqlTransaction = con.BeginTransaction("uxComexTransactionUpdateOrderItem");

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
            var SqlParam = new {@Id=Id_};

            StringBuilder strB = new StringBuilder();
            strB.Append("DELETE OrderItem WHERE Id=@Id");

            using (SqlConnection con = new SqlConnection(uxComexDbString))
            {
                con.Open();
                SqlTransaction sqlTransaction = con.BeginTransaction("uxComexTransactionDeleteOrderItem");

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

        public List<OrderItem>? List()
        {
            List<OrderItem>? ordersItems = null;
            using (SqlConnection con = new SqlConnection(uxComexDbString))
            {
                con.Open();
                ordersItems = con.Query<OrderItem>("SELECT * FROM OrderItem").ToList();
                con.Close();
            }
            return ordersItems;
        }

        public OrderItem? GetById(Int32 Id_)
        {
            var SqlParam = new { @Id = Id_ };


            OrderItem? orderItem = null;

            using (SqlConnection con = new SqlConnection(uxComexDbString))
            {
                con.Open();
                orderItem = con.Query<OrderItem>("SELECT * FROM OrderItem WHERE Id=@Id", SqlParam).FirstOrDefault();
                con.Close();
            }

            return orderItem;
        }
    }
}
