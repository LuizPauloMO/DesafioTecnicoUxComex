namespace UxComexGerenciadorPedidos.Domain.Business
{
    public class uxComexEnumerations
    {
        public uxComexEnumerations() { }
       public enum OrderStatus
        {
            NEW=0,
            PROCESSING,
            COMPLETED,
            CANCELLED
        }
    }
}
