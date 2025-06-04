using Microsoft.Data.SqlClient;
using System.Text;
using UxComexGerenciadorPedidos.Domain.Business;
using UxComexGerenciadorPedidos.Domain.Entities;
using Dapper;

namespace UxComexGerenciadorPedidos.Repositories
{
    public class ClientDb : IUxComexServiceDB<Client>
    {
        private readonly String? uxComexDbString;
        private Int32 Id;

        public ClientDb(IConfiguration configuration)
        {
            this.uxComexDbString = configuration.GetConnectionString("uxComexDbString");
        }

        public Int32 GetLastID()
        {
            return this.Id;
        }

        public async Task Add(Client client)
        {
            var SqlParams = new
            {
                Name = client.Name,
                Email = client.Email,
                Telephone = client.Telephone,
                DateRegister = client.DateRegister
            };

            StringBuilder strB = new StringBuilder();
            strB.Append("INSERT INTO Client(Name,Email,Telephone,DateRegister)").AppendLine();
            strB.Append("VALUES(@Name,@Email,@Telephone,@DateRegister)");


            using (SqlConnection con = new SqlConnection(uxComexDbString))
            {

                con.Open();
                SqlTransaction sqlTransaction = con.BeginTransaction("uxComexTransactionAddClient");

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
                Id = await con.ExecuteScalarAsync<int>("SELECT MAX(Id) FROM [Client]");
                con.Close();
            }
        }

        public async Task Update(Client client)
        {
            var SqlParams = new
            { 
                Id=client.Id,
                Name = client.Name,
                Email = client.Email,
                Telephone = client.Telephone,
                DateRegister = client.DateRegister
            };

            StringBuilder strB = new StringBuilder();
            strB.Append("UPDATE Client").AppendLine();
            strB.Append("SET Name=@Name,Email=@Email,Telephone=@Telephone,DateRegister=@DateRegister").AppendLine();
            strB.Append("WHERE Id=@Id");

            using (SqlConnection con = new SqlConnection(uxComexDbString))
            {
                con.Open();
                SqlTransaction sqlTransaction = con.BeginTransaction("uxComexTransactionAddClient");
                
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
            strB.Append("DELETE Client WHERE Id=@Id");

            using (SqlConnection con = new SqlConnection(uxComexDbString))
            {
                con.Open();
                SqlTransaction sqlTransaction = con.BeginTransaction("uxComexTransactionDeleteClient");

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

        public List<Client> ReadAll()
        {
            List<Client>? clients = null;
            using (SqlConnection con = new SqlConnection(uxComexDbString))
            {
                con.Open();
                clients = con.Query<Client>("SELECT * FROM Client").ToList();
                con.Close();
            }
            return clients;
        }

        public Client? ReadById(Int32 Id_)
        {
            var SqlParam = new {Id = Id_ };

            Client? client = null;

            using (SqlConnection con = new SqlConnection(uxComexDbString))
            {
                con.Open();
                client = con.Query<Client>("SELECT * FROM Client WHERE Id=@Id", SqlParam).FirstOrDefault();
                con.Close();
            }

            return client;
        }
    }
}
