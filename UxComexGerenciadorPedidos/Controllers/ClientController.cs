using Microsoft.AspNetCore.Mvc;
using UxComexGerenciadorPedidos.Domain.Business;
using UxComexGerenciadorPedidos.Domain.Entities;

namespace UxComexGerenciadorPedidos.Controllers
{
    public class ClientController : Controller
    {
        private readonly ClientService clientService;
        public ClientController(ClientService clientService)
        {
            this.clientService = clientService;
        }

        [Route("/Client/ListClients")]
        public IActionResult ListClients()
        {
            ViewBag.ListAllClients = clientService.ListAll();
            ViewBag.TotalItems = ViewBag.ListAllClients.Count;

            return View();
        }

        [HttpGet]
        [Route("/Client/ListClients/Filter")]

        public IActionResult ListClientFiltered(string name)
        {
            List<Client>? clients = null;
            if (System.String.IsNullOrEmpty(name))
            {
                clients = clientService.ListAll();
            }
            else
            {
                clients = clientService.ListAll()?.Where(p => p.Name.Contains(name))?.ToList();
            }

            return Json(clients);
        }

        public IActionResult NewClient()
        {
            return View();
        }

        [HttpPost]
        [Route("/Client/New")]
        public IActionResult New([FromBody] Client client)
        {
            Client? cli = clientService.GetById(client.Id);

            if (cli == null)
            {
                client.DateRegister = DateTime.Now;
                try
                {
                    clientService.New(client);
                }
                catch (Exception)
                {
                    throw;
                }               
            }

            return RedirectToAction("ListClients");
        }

        [HttpPost]
        [Route("/Client/UpdateClient")]
        public IActionResult UpdateClient([FromQuery] int Id)
        {
            Client? client = clientService.GetById(Id);
            if (client != null)
            {
                return View(client);
            }
            return View();
        }

        [HttpPost]
        [Route("/Client/Update")]
        public async Task<IActionResult> Update([FromBody] Client client)
        {
            Client? cli = clientService.GetById(client.Id);
            if (cli != null)
            {
                await clientService.Edit(client);
            }

            return RedirectToAction("ListClients");
        }

        [HttpPost]
        [Route("/Client/Delete")]
        public async Task<IActionResult> Delete([FromQuery] int Id)
        {
            Client? client = clientService.GetById(Id);
            if (client != null)
            {
                await clientService.Remove(client.Id);
            }
            return RedirectToAction("ListClients");
        }
    }
}
