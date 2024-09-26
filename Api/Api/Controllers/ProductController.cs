using Core.Entities;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{

    public class ProductController : BaseController
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }
        [Authorize]
        [HttpGet("TraerProductos")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Se produjo un error al recuperar el producto.1", error = ex.Message });
            }
        }
        [Authorize]

        [HttpGet("BuscarProducto/{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound(new { message = $"El Producto no existe: {id} not found." });
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Se produjo un error al recuperar el producto.2", error = ex.Message });
            }
        }
        [Authorize]

        [HttpPost("CrearProducto")]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Producto Invalido.", errors = ModelState });
                }

                var createdProduct = await _productService.AddProductAsync(product);
                return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProduct);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Se produjo un error al recuperar el producto.3", error = ex.Message });
            }
        }

        [HttpPut("ActualizarProducto/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {
            try
            {
                if (id != product.Id)
                {
                    return BadRequest(new { message = "No se encuentra el producto" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Invalido el producto.", errors = ModelState });
                }

                var existingProduct = await _productService.GetProductByIdAsync(id);
                if (existingProduct == null)
                {
                    return NotFound(new { message = $"Id no valido {id} not found." });
                }

                await _productService.UpdateProductAsync(product);
                return Ok(new { message = "Producto actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Se produjo un error al recuperar el producto.4", error = ex.Message });
            }
        }

        [HttpDelete("EliminarProducto/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound(new { message = $"Product with ID {id} not found." });
                }

                await _productService.DeleteProductAsync(id);
                return Ok(new { message = "Product deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Se produjo un error al recuperar el producto.4.", error = ex.Message });
            }
        }
    }
}


