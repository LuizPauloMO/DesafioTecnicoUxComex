using UxComexGerenciadorPedidos.Domain.Entities;

namespace UxComexGerenciadorPedidos.Domain.Business
{
    public interface IUxComexServiceDB<T>
    {
        Int32 GetLastID();
        public Task Add(T Table);
        public T? ReadById(Int32 Id);
        public List<T>? ReadAll();
        public Task Update(T Table);
        public Task Delete(Int32 Id);
    }
}
