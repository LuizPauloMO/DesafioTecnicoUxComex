using UxComexGerenciadorPedidos.Domain.Business;

namespace UxComexGerenciadorPedidos.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int IdClient { get; set; }
        public DateTime DateOrder { get; set; }
        public Decimal TotalValue { get; set; } = Convert.ToDecimal(0.0);
        public uxComexEnumerations.OrderStatus Status { get; set; }
       
        public String PrintStatus()
        {
            String status="";
            switch (Status)
            {
                case uxComexEnumerations.OrderStatus.NEW:
                    status = "New";
                    break;
                case uxComexEnumerations.OrderStatus.PROCESSING:
                    status = "Processing";
                    break;
                case uxComexEnumerations.OrderStatus.COMPLETED:
                    status = "Completed";
                    break;
                case uxComexEnumerations.OrderStatus.CANCELLED:
                    status = "Cancelled";
                    break;
            }

            return status;
        }
    }
}
