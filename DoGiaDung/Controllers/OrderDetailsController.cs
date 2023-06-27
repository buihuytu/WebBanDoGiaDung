using DoGiaDung.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DoGiaDung.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailsController : ControllerBase
    {
        private readonly WebbangiadungContext _context;
        public OrderDetailsController(WebbangiadungContext context)
        {
            _context = context;
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetAllOrderDetails(int id)
        {
            // query to join order and orderDetail
            var list = await _context.OrderDetails.Where(od => od.OrderId == id).ToListAsync();
            return Ok(list);
        }

        [HttpDelete("id")]
        public async Task<IActionResult> DeleteOrderDetails(int id)
        {
            var orderDetails = await _context.OrderDetails.Where(od => od.OrderId == id).ToListAsync();
            if(orderDetails.Count() == 0)
            {
                return BadRequest();
            }
            foreach(var item in orderDetails)
            {
                _context.OrderDetails.Remove(item);
            }
            await _context.SaveChangesAsync();
            return Ok("Deleted");
        }
    }
}
