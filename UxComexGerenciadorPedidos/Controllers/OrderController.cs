using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using UxComexGerenciadorPedidos.Domain.Business;
using UxComexGerenciadorPedidos.Domain.Entities;
using UxComexGerenciadorPedidos.Repositories;
using static UxComexGerenciadorPedidos.Domain.Business.uxComexEnumerations;

namespace UxComexGerenciadorPedidos.Controllers
{
    public class OrderController : Controller
    {
        private readonly OrderService orderService;

        public OrderController(OrderService orderService)
        {
            this.orderService = orderService;
        }
        [Route("/Order/ListOrders")]
        public IActionResult ListOrders()
        {
            ViewBag.ListOrderClients = orderService.Clients;
            ViewBag.ListAllOrders = orderService.ListAll();
            ViewBag.TotalItems = ViewBag.ListAllOrders.Count;
            ViewBag.ListItemsOfMenu = orderService.MenuItems;
            return View();
        }

        [HttpGet]
        [Route("/Order/ListOrders/Filter")]

        public IActionResult ListOrdersFiltered([FromQuery] string key, [FromQuery] OrderStatus status)
        {
            List<Order>? orders = null;
            String key_ = key?.Trim() ?? String.Empty;

            if (System.String.IsNullOrEmpty(key?.Trim() ?? String.Empty))
            {
                orders = orderService.ListAll();
            }
            else
            {
                List<Client>? clientsFiltered = orderService.Clients?
                                   .Where(p => p.Name.Contains(key_)).ToList();

                if (clientsFiltered != null)
                {
                    switch (status)
                    {
                        case OrderStatus.NEW:
                            orders = orderService.ListAll()?
                                               .Where(p => clientsFiltered
                                               .Select(p => p.Id)
                                               .Contains(p.IdClient))
                                               .Where(p => p.Status == OrderStatus.NEW)
                                               .ToList<Order>();

                            break;
                        case OrderStatus.PROCESSING:
                            orders = orderService.ListAll()?
                                              .Where(p => clientsFiltered
                                              .Select(p => p.Id)
                                              .Contains(p.IdClient))
                                              .Where(p => p.Status == OrderStatus.PROCESSING)
                                              .ToList<Order>();
                            break;
                        case OrderStatus.COMPLETED:
                            orders = orderService.ListAll()?
                                             .Where(p => clientsFiltered
                                             .Select(p => p.Id)
                                             .Contains(p.IdClient))
                                             .Where(p => p.Status == OrderStatus.COMPLETED)
                                             .ToList<Order>();
                            break;
                        case OrderStatus.CANCELLED:
                            orders = orderService.ListAll()?
                                             .Where(p => clientsFiltered
                                             .Select(p => p.Id)
                                             .Contains(p.IdClient))
                                             .Where(p => p.Status == OrderStatus.CANCELLED)
                                             .ToList<Order>();
                            break;
                        default:
                            orders = orderService.ListAll()?
                                            .Where(p => clientsFiltered
                                            .Select(p => p.Id)
                                            .Contains(p.IdClient))
                                            .Where(p => p.Status == OrderStatus.CANCELLED)
                                            .ToList<Order>();
                            break;
                    }
                }
            }

            List<object> json = new List<object>();

            if (orders != null)
            {
                foreach (Order order in orders)
                {
                    json.Add(new
                    {
                        Id = order.Id,
                        IdClient = order.IdClient,
                        TotalValue = order.TotalValue,
                        Status = order.PrintStatus(),
                        DateOrder = order.DateOrder,
                        ClientName = orderService.GetClientByID(order.IdClient)?.Name ?? String.Empty
                    });
                }
            }

            return Json(json);
        }

        [Route("/Order/NewOrder")]
        public IActionResult NewOrder()
        {
            orderService.Clear();
            ViewBag.ListOrderClients = orderService.Clients;
            ViewBag.ListOrderProducts = orderService.Products;
            ViewBag.ListItemsOfMenu = orderService.MenuItems;

            return View();
        }

        [HttpPost]
        [Route("/Order/NewOrderItem")]
        public IActionResult NewOrderItem([FromBody] OrderItem orderItem)
        {
            Product? prod = orderService.GetProductByID(orderItem.IdProduct);
            List<object> json = new List<object>();

            if (prod != null)
            {
                Int32 currentStock = prod.QuantityOfStock;
                foreach (OrderItem item in orderService.MenuItems.Where(p => p.IdProduct == prod.Id))
                {
                    currentStock -= item.Quantity;
                }

                if (currentStock >= orderItem.Quantity)
                {
                    orderItem.UnityPrice = prod?.Price ?? new decimal(0.0);
                    orderService.IncludeItemToOrderMenu(orderItem);
                    orderService.CalculateTotalValueOfMenu();
                    ViewBag.IsStockEmpty = false;
                }
                else
                {
                    ViewBag.IsStockEmpty = true;

                    return Json(new
                    {
                        ProductName = prod?.Name,
                        Quantity = orderItem.Quantity,
                        QuantityOfStock = currentStock,
                        QuantityOfStockIsEmpty = ViewBag.IsStockEmpty
                    });
                }
            }


            foreach (OrderItem item in orderService.MenuItems)
            {
                Product? product = orderService.GetProductByID(item.IdProduct);

                json.Add(new
                {
                    ProductName = product?.Name,
                    Quantity = item.Quantity,
                    UnityPrice = product?.Price,
                    QuantityOfStockIsEmpty = ViewBag.IsStockEmpty
                });
            }

            ViewBag.TotalValueOrder = orderService.TotalValue;
            return Json(json);
        }

        [HttpPost]
        [Route("/Order/Save")]
        public async Task<IActionResult> Save([FromBody] int idClient)
        {
            Client? cli = orderService.GetClientByID(idClient);

            if (cli != null)
            {
                await orderService.Save(cli);
            }

            orderService.Clear();

            return Redirect("/Order/ListOrders");
        }
        [HttpPost]
        [Route("/Order/UpdateOrder")]
        public IActionResult UpdateOrder([FromQuery] int Id)
        {
            ViewBag.ListOrderClients = orderService.Clients;
            ViewBag.ListOrderProducts = orderService.Products;
                   
            Order? order = orderService.GetById(Id);

            if (order != null)
            {
                List<OrderItem>? orderItems = orderService.GetItemsOfOrder(order.Id);
                if(orderItems != null)
                {
                    foreach (OrderItem item in orderItems)
                    {
                        orderService.IncludeItemToOrderMenu(item);
                    }
                }

                ViewBag.TotalValueOrder = orderService.CalculateTotalValueOfMenu();
                return View(order);
            }
            return View();
        }
       
        public void Delete([FromBody] int Id)
        {
            orderService.Delete(Id);

            RedirectToAction("ListOrders");
        }
    }
}
