using DoGiaDung.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoGiaDung.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly WebbangiadungContext _context;
        public ContactsController(WebbangiadungContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllContacts()
        {
            var countTrash = _context.Contacts.Where(m => m.Status == 0).Count();
            var list = await _context.Contacts.Where(m => m.Status != 0).ToListAsync();
            return Ok(new { countTrash, list });
        }

        [HttpGet]
        [Route("Trash")]
        public async Task<IActionResult> GetTrash()
        {
            var list = await _context.Contacts.Where(m => m.Status == 0).ToListAsync();
            return Ok(list);
        }

        [HttpDelete("id")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return BadRequest();
            }
            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();
            return Ok("Deleted");
        }
    }
    
}
