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
            return ctx.List();
        }

        public List<Client>? ListByName(String name)
        {
            return ctx.List()?
                             .Where(p => p.Name == name)
                             .ToList();
        }

        public Client? GetById(Int32 Id)
        {
            return ctx.GetById(Id);
        }

        public List<Client>? ListAllInOrderByAsc()
        {
            return ListAll()?
                            .OrderBy(p => p.Name)
                            .ToList();
        }

        public List<Client>? ListAllInOrderByDesc()
        {
            return ListAll()?
                            .OrderByDescending(p => p.Name)
                            .ToList();
        }
        public void New(Client? client)
        {
            if (client != null)
            {
                Client? client_ = ctx.List()?
                                            .Where(p => p.Name == client.Name)
                                            .FirstOrDefault();
                if (client_ == null)
                {
                    try
                    {
                        ctx.Add(client);
                    }
                    catch (Exception)
                    {
                        throw;
                    }                  
                }
            }
        }

        public async Task Remove(Int32 Id)
        {
            if (Id > 0)
            {
                try
                {
                    await ctx.Delete(Id);
                }
                catch (Exception)
                {
                    throw;
                }                
            }
        }

        public async Task Edit(Client? client)
        {
            if (client != null)
            {
                Client? cli = ctx.GetById(client.Id);

                if (cli != null)
                {
                    try
                    {
                        await ctx.Update(client);
                    }
                    catch (Exception)
                    {
                        throw;
                    }                   
                }
            }
        }

        public Client? Search(Client client)
        {
            return ListByName(client.Name)?.FirstOrDefault();
        }
    }
}
