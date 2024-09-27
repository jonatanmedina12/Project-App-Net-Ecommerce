using Core.DTOs;
using Core.Entities;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{

    public class OrderController : BaseController
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }
        [Authorize]
        [HttpGet("TraerOrdenes")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Se produjo un error al recuperar orden", error = ex.Message });
            }
        }
        [Authorize]

        [HttpGet("BuscarOrden/{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                {
                    return NotFound(new { message = $"orden con ID {id} no encontrado." });
                }
                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Se produjo un error al recuperar orden", error = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("CrearOrden")]
        public async Task<ActionResult<Order>> CreateOrder(Order order)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Datos de pedido no válidos.", errors = ModelState });
                }

                var createdOrder = await _orderService.CreateOrderAsync(order);
                return CreatedAtAction(nameof(GetOrder), new { id = createdOrder.Id }, createdOrder);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Se produjo un error al recuperar orden", error = ex.Message });
            }
        }
        [Authorize]

        [HttpPut("ActualizarOrden/{id}")]
        public async Task<IActionResult> UpdateOrder(int id, Order order)
        {
            try
            {
                // Validación del modelo
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Invalido la orden.", errors = ModelState });
                }

                // Obtener la orden existente
                var existingOrder = await _orderService.GetOrderByIdAsync(id);
                if (existingOrder == null)
                {
                    return NotFound(new { message = $"Id no valido {id} not found." });
                }

                // Actualizar las propiedades de la orden
                existingOrder.OrderDate = DateTime.Now;
                existingOrder.CustomerEmail = order.CustomerEmail;
                existingOrder.TotalAmount = order.TotalAmount;

                // Manejar los elementos de la orden
                foreach (var item in order.Items)
                {
                    // Verificar si el ítem existe
                    var existingItem = existingOrder.Items.FirstOrDefault(i => i.Id == item.Id);
                    if (existingItem != null)
                    {
                        // Actualizar las propiedades del ítem existente
                        existingItem.Price = item.Price; 
                        existingItem.Quantity = item.Quantity; 
                        
                                                                 // ... asigne otras propiedades según sea necesario
                    }
                    else
                    {
                        // Agregar nuevo ítem si no existe
                        existingOrder.Items.Add(item);
                    }
                }

                // Guardar cambios en la base de datos
                await _orderService.UpdateOrderAsync(existingOrder);
                return Ok(new { message = "Orden actualizada correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Se produjo un error al actualizar la orden.", error = ex.Message });
            }
        }
        [Authorize]

        [HttpDelete("EliminarOrden/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var product = await _orderService.GetOrderByIdAsync(id);
                if (product == null)
                {
                    return NotFound(new { message = $"orden with ID {id} not found." });
                }

                await _orderService.DeleteOrderAsync(id);
                return Ok(new { message = "orden deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Se produjo un error al recuperar  la orden.4.", error = ex.Message });
            }
        }
    }
}

