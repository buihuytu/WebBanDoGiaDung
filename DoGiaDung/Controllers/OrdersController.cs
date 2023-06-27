using DoGiaDung.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DoGiaDung.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly WebbangiadungContext _context;
        public OrdersController(WebbangiadungContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var countTrash = _context.Orders.Where(m => m.Trash == 1).Count();
            // query to join order and orderDetail
            var results = await (from o in _context.Orders
                           join od in _context.OrderDetails on o.Id equals od.OrderId
                           where o.Trash != 1

                           group od by new 
                           { 
                               od.OrderId, 
                               o.CreateDate,
                               o.DeliveryName,
                               o.Status,
                               o.ExportDate
                           } into groupb
                           orderby groupb.Key.CreateDate descending
                           select new ListOrder
                           {
                               ID = groupb.Key.OrderId,
                               SAmount = groupb.Sum(m => m.Amount),
                               CustomerName = groupb.Key.DeliveryName,
                               Status = groupb.Key.Status,
                               CreateDate = groupb.Key.CreateDate,
                               ExportDate = groupb.Key.ExportDate,

                           }).ToListAsync();

            return Ok(new { countTrash, results });
        }

        [HttpGet]
        [Route("Trash")]
        public async Task<IActionResult> GetAllTrash()
        {
            // query to join order and orderDetail
            var results = await (from o in _context.Orders
                                 join od in _context.OrderDetails on o.Id equals od.OrderId
                                 where o.Trash == 1

                                 group od by new
                                 {
                                     od.OrderId,
                                     o.CreateDate,
                                     o.DeliveryName,
                                     o.Status,
                                     o.ExportDate
                                 } into groupb
                                 orderby groupb.Key.CreateDate descending
                                 select new ListOrder
                                 {
                                     ID = groupb.Key.OrderId,
                                     SAmount = groupb.Sum(m => m.Amount),
                                     CustomerName = groupb.Key.DeliveryName,
                                     Status = groupb.Key.Status,
                                     CreateDate = groupb.Key.CreateDate,
                                     ExportDate = groupb.Key.ExportDate,

                                 }).ToListAsync();

            return Ok(results);
        }

        [HttpDelete("id")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return BadRequest();
            }
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return Ok("Deleted");
        }
    }
}
