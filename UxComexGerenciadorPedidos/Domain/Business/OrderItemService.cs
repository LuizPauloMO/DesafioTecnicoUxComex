using UxComexGerenciadorPedidos.Domain.Entities;
using UxComexGerenciadorPedidos.Repositories;

namespace UxComexGerenciadorPedidos.Domain.Business
{
    public class OrderItemService
    {
        private readonly IUxComexServiceDB<OrderItem> ctxOrderItem;
        private readonly IConfiguration configuration;
        public List<OrderItem>? OrderItems { get; }

        public OrderItemService(IConfiguration configuration)
        {
            this.configuration = configuration;
            ctxOrderItem = new OrderItemDb(configuration);
        }

        public List<OrderItem>? List()
        {
            return ctxOrderItem.List();
        }

        public OrderItem? GetById(Int32 Id)
        {
            return ctxOrderItem.GetById(Id);
        }

        public void Add(OrderItem orderItem)
        {
            try
            {
                ctxOrderItem.Add(orderItem);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void Delete(Int32 Id)
        {
            try
            {
                ctxOrderItem.Delete(Id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool DeleteInCascade(Int32 OrderId)
        {
            bool IS_SUCCESS = true;
            List<OrderItem>? orderItems = List()?
                                                .Where(p => p.IdOrder == OrderId).ToList();

            if (orderItems != null)
            {
                foreach (OrderItem item in orderItems)
                {
                    try
                    {
                        Delete(item.Id);
                        IS_SUCCESS &= true;
                    }
                    catch (Exception)
                    {
                        IS_SUCCESS = false;
                        throw;
                    }
                }
            }

            return IS_SUCCESS;
        }

        public Decimal RecalculateTotalValueOfOrderItems(Int32 OrderId)
        {
            Decimal total = new decimal(0.0);
            
            List<OrderItem>? orderItems = List()?
                                                .Where(p=>p.IdOrder== OrderId)
                                                .ToList();

            if (orderItems != null)
            {
                foreach (OrderItem item in orderItems)
                {
                    total += item.UnityPrice * item.Quantity;
                }
            }

            return total;
        }
    }
}
