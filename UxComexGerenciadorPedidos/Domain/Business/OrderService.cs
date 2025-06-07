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
        private readonly ClientService clientService;
        private readonly ProductService productService;
        private readonly OrderItemService orderItemService;

        public List<Product>? Products { get; }
        public List<Client>? Clients { get; }
        public List<OrderItem> MenuItems { get; }
        public Decimal TotalValue { get; set; }
        public OrderService(IConfiguration configuration, ClientService clientService, ProductService productService, OrderItemService orderItemService)
        {
            this.ctxOrder = new OrderDb(configuration);
            this.clientService = clientService;
            this.productService = productService;
            this.orderItemService = orderItemService;
            this.MenuItems = new List<OrderItem>();
            this.Products = productService.ListAll();
            this.Clients = clientService.ListAll();
        }

        #region C.R.U.D
        public List<Order>? ListAll()
        {
            return ctxOrder.List();
        }

        public List<Order>? ListOrderByDate(DateTime date)
        {
            return ctxOrder.List()?.Where(p => p.DateOrder.ToShortDateString() == date.ToShortDateString()).ToList();
        }

        public Order? GetById(Int32 Id)
        {
            return ctxOrder.GetById(Id);
        }

        public List<OrderItem>? GetItemsOfOrder(Int32 Id)
        {
            List<OrderItem>? orderItems = null;
            Order? order = ctxOrder.GetById(Id);
            
            if(order != null)
            {
                orderItems=orderItemService.List()?
                                                  .Where(p=>p.IdOrder==order.Id)
                                                  .ToList();
            }

            return orderItems;
        }
        public void Delete(Int32 Id)
        {
            bool IsDeleteAllOrderItemsInCascade = true;
            Order? order = ctxOrder.GetById(Id);
            if (order != null)
            {
                IsDeleteAllOrderItemsInCascade = orderItemService.DeleteInCascade(order.Id);

                if (IsDeleteAllOrderItemsInCascade)
                {
                    ctxOrder.Delete(order.Id);
                }
            }
        }

        public void Update(Order? order)
        {
            if (order != null)
            {
                Order? order_ = ctxOrder.GetById(order.Id);
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

        public void AlterOrderStatus(Order order, OrderStatus orderStatus)
        {
            Order? order_ = this.ctxOrder.GetById(order.Id);
            if (order_ != null)
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
        }

        public void IncludeItemToOrderMenu(OrderItem orderItem)
        {
            //if (orderItem != null)
                MenuItems.Add(orderItem);
        }

        public Decimal CalculateTotalValueOfMenu()
        {
            Decimal total = new decimal(0.0);

            foreach (OrderItem item in MenuItems)
            {
                total += item.UnityPrice * item.Quantity;
            }

            this.TotalValue = total;
            return this.TotalValue;
        }

        public async Task Save(Client client)
        {
            Client? cli = clientService.GetById(client.Id);

            if (cli != null)
            {
                Int32 OrderId = 0;
                Order order = new Order();
                order.IdClient = cli.Id;
                order.DateOrder = DateTime.Now;
                order.Status = OrderStatus.NEW;
                order.TotalValue = this.TotalValue;
                bool OrderCreatedWithSuccess = false;
                try
                {                  
                    ctxOrder.Add(order);
                    OrderCreatedWithSuccess = true;
                }
                catch (Exception)
                {
                    throw;
                }

                if (OrderCreatedWithSuccess)
                {
                    OrderId = ctxOrder.GetLastID();

                    Order? orderNew = ctxOrder.GetById(OrderId);

                    if (orderNew != null)
                    {
                        foreach (OrderItem item in MenuItems)
                        {

                            item.IdOrder = orderNew.Id;

                            try
                            {
                                orderItemService.Add(item);
                                await productService.RemoveProductOfStock(item.IdProduct, item.Quantity);
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                        }

                        orderNew.TotalValue = orderItemService.RecalculateTotalValueOfOrderItems();

                        await ctxOrder.Update(orderNew);
                    }
                }
            }
        }

        public void Clear()
        {
            MenuItems.Clear();
        }
    }
}
