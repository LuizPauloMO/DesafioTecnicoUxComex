using UxComexGerenciadorPedidos.Domain.Entities;

namespace UxComexGerenciadorPedidos.Domain.Business
{
    public interface IUxComexServiceDB<T>
    {
        Int32 GetLastID();
        public void Add(T Table);
        public T? GetById(Int32 Id);
        public List<T>? List();
        public Task Update(T Table);
        public Task Delete(Int32 Id);
    }
}
