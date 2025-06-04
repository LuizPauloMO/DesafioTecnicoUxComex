using Microsoft.EntityFrameworkCore;
using UxComexGerenciadorPedidos.Domain.Entities;

namespace UxComexGerenciadorPedidos.Infraestructure
{
    public class UxComexDataContext:DbContext
    {
        public UxComexDataContext(DbContextOptions<UxComexDataContext> options) : base(options) { }
        
        DbSet<Client> Clients { get; set; }
        DbSet<Product> Products { get; set; }
        DbSet<Order> Orders { get; set; }
        DbSet<OrderItem> OrderItems { get; set; }
    }
}
