using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Text.Json.Serialization;
using UxComexGerenciadorPedidos.Domain.Business;
using UxComexGerenciadorPedidos.Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UxComexGerenciadorPedidos.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductService productService;
        public ProductController(ProductService productService)
        {
            this.productService = productService;
        }

        public IActionResult ListProducts()
        {
            ViewBag.ListAllProducts = productService.ListAll();
            ViewBag.TotalItems = ViewBag.ListAllProducts.Count;

            return View();
        }

        [HttpGet]
        [Route("/Product/ListProducts/Filter")]
      
        public IActionResult ListProductsFiltered(string name)
        {
            List<Product>? products = null;
            if (System.String.IsNullOrEmpty(name))
            {
                products = productService.ListAll();
            }
            else
            {
                products= productService.ListAll()?.Where(p => p.Name.Contains(name))?.ToList();
            }

            return Json(products);
        }

        [Route("/Product/NewProduct")]
        public IActionResult NewProduct()
        {
            return View();
        }

        [Route("/Product/EditProduct")]
        public IActionResult EditProduct()
        {
            return View();
        }

        [HttpPost]
        [Route("/Product/New")]
        public async Task<IActionResult> New([FromBody] Product product)
        {
           await productService.Create(product);

           return RedirectToAction("ListProducts");
        }

        [HttpPost]
        [Route("/Product/UpdateProduct")]
        public async Task<IActionResult> Update([FromQuery]int Id)
        {
            Product? product = productService.GetById(Id);
            if(product != null)
            {
                await productService.Update(product);
            }
            
            return RedirectToAction("ListProducts");
        }
        [HttpPost]
        [Route("/Product/Delete")]
        public async Task<IActionResult> Delete([FromQuery] int Id)
        {
            Product? product = productService.GetById(Id);
            if (product != null)
            {
                await productService.Delete(product.Id);
            }
           
           return RedirectToAction("ListProducts");
        }
    }
}
