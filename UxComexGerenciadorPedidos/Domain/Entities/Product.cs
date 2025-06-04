using System.ComponentModel.DataAnnotations.Schema;

namespace UxComexGerenciadorPedidos.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public String Name { get; set;}
        public String? Description { get; set; }
        public Decimal Price { get; set; }
        public Int32 QuantityOfStock { get; set; }
    }
}
