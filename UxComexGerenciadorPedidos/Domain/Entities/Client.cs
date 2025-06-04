namespace UxComexGerenciadorPedidos.Domain.Entities
{
    public class Client
    {
        public int Id { get; set; }
        public String Name { get; set;}
        public String? Email { get; set;}
        public String? Telephone { get; set; }
        public DateTime DateRegister { get; set;}
    }
}
