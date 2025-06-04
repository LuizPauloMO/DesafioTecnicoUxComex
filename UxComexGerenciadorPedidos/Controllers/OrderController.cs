using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using UxComexGerenciadorPedidos.Domain.Business;
using UxComexGerenciadorPedidos.Domain.Entities;
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
            ViewBag.ListOrderItems = orderService.ListOrderItems();
            return View();
        }

        [HttpGet]
        [Route("/Order/ListOrders/Filter")]

        public IActionResult ListOrdersFiltered(int idclient, OrderStatus orderstatus)
        {
            List<Order>? orders = null;
            if (idclient == 0)
            {
                orders = orderService.ListAll();
            }
            else
            {
                Client? client = orderService.Clients?
                                             .Where(p => p.Id == idclient)
                                             .FirstOrDefault();

                if (client != null)
                {
                    switch (orderstatus)
                    {
                        case OrderStatus.NEW:
                            orders = orderService.ListAll()?.Where(p => p.IdClient == client.Id && p.Status == OrderStatus.NEW).ToList();
                            break;
                        case OrderStatus.PROCESSING:
                            orders = orderService.ListAll()?.Where(p => p.IdClient == client.Id && p.Status == OrderStatus.PROCESSING).ToList();
                            break;
                        case OrderStatus.COMPLETED:
                            orders = orderService.ListAll()?.Where(p => p.IdClient == client.Id && p.Status == OrderStatus.COMPLETED).ToList();
                            break;
                        case OrderStatus.CANCELLED:
                            orders = orderService.ListAll()?.Where(p => p.IdClient == client.Id && p.Status == OrderStatus.CANCELLED).ToList();
                            break;
                        default:
                            break;
                    }
                }
            }

            return Json(orders);
        }

        [Route("/Order/NewOrder")]
        public IActionResult NewOrder()
        {
            orderService.Clear();
            ViewBag.ListOrderClients = orderService.Clients;
            ViewBag.ListOrderProducts = orderService.Products;
            ViewBag.ListOrderItems = orderService.OrderItems;
           
            return View();
        }

        [HttpPost]
        [Route("/Order/NewOrderItem")]
        public IActionResult NewOrderItem([FromBody] OrderItem orderItem)
        {
            Product? prod = orderService.GetProductByID(orderItem.IdProduct);

            orderItem.UnityPrice = prod?.Price??new decimal(0.0);
            orderService.IncludeItemInMyOrder(orderItem);
            orderService.CalculateTotalValue();

            List<object> json = new List<object>();
               foreach(OrderItem item in orderService.OrderItems)
               {
                Product? product = orderService.GetProductByID(item.IdProduct);

                json.Add(new
                {
                    ProductName = product?.Name,
                    Quantity = item.Quantity,
                    UnityPrice = product?.Price
                }) ;
               }
            
               ViewBag.TotalValueOrder = orderService.ValueTotal;
               return Json(json);
        }

        [HttpPost]
        [Route("/Order/Save")]
        public IActionResult Save([FromBody]int idClient)
        {
            Int32 nItems=orderService.OrderItems.Count();
            Client? cli = orderService.GetClientByID(idClient);

            if(nItems > 0 && cli != null)
            {
                orderService.Save(cli);
            }

            orderService.Clear();

           return Redirect("/Order/ListOrders");
        }
        public void Edit([FromBody] Order order)
        {
            orderService.Update(order);

            Redirect("/Order/ListOrders");
        }

        public void Delete([FromBody] int Id)
        {
            orderService.Delete(Id);

            RedirectToAction("ListOrders");
        }
    }
}
