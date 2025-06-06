﻿using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Net.Sockets;
using System.Text;
using UxComexGerenciadorPedidos.Domain.Business;
using UxComexGerenciadorPedidos.Domain.Entities;
using static UxComexGerenciadorPedidos.Domain.Business.uxComexEnumerations;

namespace UxComexGerenciadorPedidos.Repositories
{
    public class OrderDb : IUxComexServiceDB<Order>
    {
        private readonly String? uxComexDbString;
        private Int32 Id;

        public OrderDb(IConfiguration configuration)
        {
            this.uxComexDbString = configuration.GetConnectionString("uxComexDbString");
            this.Id = 0;
        }

        public Int32 GetLastID()
        {
            return this.Id;
        }
        public async Task Add(Order order)
        {
            var SqlParams = new
            {
                IdClient = order.IdClient,
                ValueTotal = order.ValueTotal,
                DateOrder = order.DateOrder,
                OrderStatus = order.Status
            };

            StringBuilder strB = new StringBuilder();
            strB.Append("INSERT INTO [Order](IdClient,ValueTotal,DateOrder,Status)").AppendLine();
            strB.Append("VALUES(@IdClient,@ValueTotal,@DateOrder,@OrderStatus)");


            using (SqlConnection con = new SqlConnection(uxComexDbString))
            {
                con.Open();
                SqlTransaction sqlTransaction = con.BeginTransaction("uxComexTransactionAddOrder");

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
                Id = await con.ExecuteScalarAsync<int>("SELECT MAX(Id) FROM [Order]");
                con.Close();
            }
        }

        public async Task Update(Order order)
        {
            var SqlParams = new
            {
                Id = order.Id,
                IdClient = order.IdClient,
                ValueTotal = order.ValueTotal,
                DateOrder = order.DateOrder,
                OrderStatus = order.Status
            };

            StringBuilder strB = new StringBuilder();
            strB.Append("UPDATE [Order]").AppendLine();
            strB.Append("SET IdClient=@IdClient,ValueTotal=@ValueTotal,DateOrder=@DateOrder,Status=@OrderStatus").AppendLine();
            strB.Append("WHERE Id=@Id");

            using (SqlConnection con = new SqlConnection(uxComexDbString))
            {
                con.Open();
                SqlTransaction sqlTransaction = con.BeginTransaction("uxComexTransactionAddOrder");

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
            strB.Append("DELETE [Order] WHERE Id=@Id");

            using (SqlConnection con = new SqlConnection(uxComexDbString))
            {
                con.Open();
                SqlTransaction sqlTransaction = con.BeginTransaction("uxComexTransactionDeleteOrder");

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

        public List<Order>? ReadAll()
        {
            List<Order>? orders = null;
            using (SqlConnection con = new SqlConnection(uxComexDbString))
            {
                con.Open();
                orders = con.Query<Order>("SELECT * FROM [Order]").ToList();
                con.Close();
            }
            return orders;
        }

        public Order? ReadById(Int32 Id_)
        {
            var SqlParam = new { Id = Id_ };

            Order? order = null;

            using (SqlConnection con = new SqlConnection(uxComexDbString))
            {
                con.Open();
                order = con.Query<Order>("SELECT * FROM [Order] WHERE Id=@Id", SqlParam).FirstOrDefault();
                con.Close();
            }

            return order;
        }
    }
}
