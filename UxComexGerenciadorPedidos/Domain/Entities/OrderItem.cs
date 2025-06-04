namespace UxComexGerenciadorPedidos.Domain.Entities
{
    public class OrderItem
    {
        public Int32 Id { get; set; }
        public Int32 IdOrder { get; set; }
        public Int32 IdProduct { get; set; }
        public Decimal UnityPrice { get; set;}
        public Int32 Quantity { get; set; } = 0;
        List<Product>? Products { get; set;}
        List<Order>? Orders { get; set; }
    }
}
