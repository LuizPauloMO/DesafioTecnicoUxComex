using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Runtime.InteropServices;
using UxComexGerenciadorPedidos.Domain.Entities;
using UxComexGerenciadorPedidos.Infraestructure;
using UxComexGerenciadorPedidos.Repositories;

namespace UxComexGerenciadorPedidos.Domain.Business
{
    public class ClientService
    {
        private IUxComexServiceDB<Client> ctx;

        public ClientService(IConfiguration conf)
        {
            this.ctx = new ClientDb(conf);
        }
        public List<Client>? ListAll()
        {
            return ctx.ReadAll();
        }

        public List<Client>? ListByName(String name)
        {
            return ctx.ReadAll()?.Where(p => p.Name == name).ToList();
        }

        public Client? GetById(Int32 Id)
        {
            return ctx.ReadById(Id);
        }

        public List<Client>? ListAllInOrderByAsc()
        {
            return ListAll()?.OrderBy(p => p.Name).ToList();
        }

        public List<Client>? ListAllInOrderByDesc()
        {
            return ListAll()?.OrderByDescending(p => p.Name).ToList();
        }
        public async Task New(Client? client)
        {
            if (client != null)
            {
                Client? client_ = ctx.ReadAll()?.Where(p => p.Name == client.Name).FirstOrDefault();
                if (client_ == null)
                {
                   await ctx.Add(client);
                }
            }
        }

        public async Task Remove(Int32 Id)
        {
            if (Id > 0)
            {
                await ctx.Delete(Id);
            }
        }

        public async Task Edit(Client? client)
        {
            if (client != null)
            {
                Client? cli = ctx.ReadById(client.Id);

                if (cli != null)
                {
                   await ctx.Update(client);
                }
            }
        }

        public Client? Search(Client client)
        {
            return ListByName(client.Name)?.FirstOrDefault();
        }
    }
}
