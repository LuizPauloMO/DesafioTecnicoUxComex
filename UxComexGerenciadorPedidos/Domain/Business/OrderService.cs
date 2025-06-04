using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.InteropServices;
using UxComexGerenciadorPedidos.Domain.Entities;
using UxComexGerenciadorPedidos.Infraestructure;
using UxComexGerenciadorPedidos.Repositories;
using static UxComexGerenciadorPedidos.Domain.Business.uxComexEnumerations;

namespace UxComexGerenciadorPedidos.Domain.Business
{
    public class OrderService
    {
        private readonly IUxComexServiceDB<Order> ctxOrder;
        private readonly IUxComexServiceDB<OrderItem> ctxOrderItem;
        private readonly ClientService clientService;
        private readonly ProductService productService;
        private readonly List<OrderItem> items;
        public List<Product>? Products { get; }
        public List<Client>? Clients { get; }
        public List<OrderItem> OrderItems { get; }
        public Decimal ValueTotal { get; set; }
        public OrderService(IConfiguration configuration, ClientService clientService, ProductService productService)
        {
            this.ctxOrder = new OrderDb(configuration);
            this.ctxOrderItem = new OrderItemDb(configuration);
            this.clientService = clientService;
            this.productService = productService;
            this.items = new List<OrderItem>();
            this.Products = productService.ListAll();
            this.Clients = clientService.ListAll();
            OrderItems = items;
        }

        #region C.R.U.D
        public List<Order>? ListAll()
        {
            return ctxOrder.ReadAll();
        }

        public List<OrderItem>? ListOrderItems()
        {
            return this.OrderItems;
        }

        private List<OrderItem>? ListProductsByOrderId(int id)
        {
            return ListOrderItems()?
                                   .Where(p => p.IdOrder == id)
                                   .ToList();
        }

        public List<Order>? ListOrderByDate(DateTime date)
        {
            return ctxOrder.ReadAll()?.Where(p => p.DateOrder.ToShortDateString() == date.ToShortDateString()).ToList();
        }

        public Order? ListById(Int32 Id)
        {
            return ctxOrder.ReadById(Id);
        }


        public void Delete(Int32 Id)
        {
            Int32 orderItemId = 0;
            Order? order_ = ctxOrder.ReadById(Id);
            if (order_ != null)
            {
                OrderItem? orderItem = ctxOrderItem.ReadAll()?
                                                 .Where(p => p.IdOrder == order_.Id)
                                                 .FirstOrDefault();

                if (orderItem != null)
                {
                    orderItemId = orderItem.Id;
                    ctxOrderItem.Delete(orderItemId);
                }

                orderItem = ctxOrderItem.ReadAll()?
                                        .Where(p => p.IdOrder == orderItemId)
                                        .FirstOrDefault();

                if (orderItem == null)
                {
                    ctxOrder.Delete(order_.Id);
                }
            }
        }

        public void Update(Order? order)
        {
            if (order != null)
            {
                Order? order_ = ctxOrder.ReadById(order.Id);
                if (order_ != null)
                {
                    ctxOrder.Update(order);
                }
            }
        }
        #endregion

        public Product? GetProductByID(Int32 Id)
        {
            return productService.GetById(Id);
        }

        public List<Product>? GetProductsByName(String name)
        {
            return productService.GetByName(name);
        }

        public Client? GetClientByID(Int32 Id)
        {
            return clientService.GetById(Id);
        }

        public OrderStatus GetOrderStatus()
        {
            return OrderStatus.NEW;
        }

        public void ChangeOrderStatus(Order order, OrderStatus orderStatus)
        {
            try
            {
                order.Status = orderStatus;
                ctxOrder.Update(order);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void IncludeItemInMyOrder(OrderItem orderItem)
        {
            items.Add(orderItem);         
        }

        public void CalculateTotalValue()
        {
            Decimal total = new decimal(0.0);

            foreach (OrderItem item in items)
            {
                total += item.UnityPrice * item.Quantity;
            }

            this.ValueTotal= total;
        }

        public void Save(Client client)
        {
            Client? cli = clientService.GetById(client.Id);

            if (cli != null)
            {
                Int32 OrderId = 0;

                try
                {
                    OrderId = ctxOrder.GetLastID();
                    Order order = new Order();
                    order.IdClient = cli.Id;
                    order.DateOrder = DateTime.Now;
                    order.Status = OrderStatus.NEW;
                    order.ValueTotal = this.ValueTotal;

                    ctxOrder.Add(order);
                }
                catch (Exception)
                {
                    OrderId = 0;
                    throw;
                }

                Order? orderNew = ctxOrder.ReadById(OrderId+1);

                if (orderNew != null)
                {
                    foreach (OrderItem item in items)
                    {
                        item.IdOrder = orderNew.Id;

                        try
                        {
                            ctxOrderItem.Add(item);
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                }
            }
        }

        public void Clear()
        {
            this.items.Clear();
        }
    }
}
