using DoGiaDung.Library;
using DoGiaDung.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoGiaDung.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicsController : ControllerBase
    {
        private readonly WebbangiadungContext _context;
        public TopicsController(WebbangiadungContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var countTrash = await _context.Topics.Where(m => m.Status == 0).CountAsync();
            var list = await _context.Topics.Where(m => m.Status != 0).ToListAsync();

            foreach (var row in list)
            {
                var temp_link = _context.Links.Where(m => m.Type == "topic" && m.TableId == row.Id);
                if (temp_link.Count() > 0)
                {
                    var row_link = temp_link.First();
                    row_link.Name = row.Name;
                    row_link.Slug = row.Slug;
                    _context.Entry(row_link).State = EntityState.Modified;
                }
                else
                {
                    var row_link = new Link();
                    row_link.Name = row.Name;
                    row_link.Slug = row.Slug;
                    row_link.Type = "topic";
                    row_link.TableId = row.Id;
                    _context.Links.Add(row_link);
                }
            }
            _context.SaveChanges();
            return Ok(new { countTrash, list });
        }

        [HttpGet]
        [Route("Trash")]
        public async Task<IActionResult> GetTrash()
        {
            var list = await _context.Topics.Where(m => m.Status == 0).ToListAsync();
            return Ok(list);
        }

        [HttpDelete("id")]
        public async Task<IActionResult> DeleteTopic(int id)
        {
            var topic = await _context.Topics.FindAsync(id);
            if(topic == null)
            {
                return BadRequest();
            }
            _context.Topics.Remove(topic);
            await _context.SaveChangesAsync();
            return Ok("Deleted");
        }

        [HttpPost]
        public async Task<IActionResult> InsertTopic(Topic topic)
        {
            if(ModelState.IsValid)
            {
                if(topic.ParentId == null)
                {
                    topic.ParentId = 0;
                }
                String slug = XString.ToAscii(topic.Name);
                if(_context.Categories.Where(c => c.Slug == slug).Count() > 0)
                {
                    return BadRequest("Tên danh mục đã tồn tại, vui lòng thử lại!");
                }
                if (_context.Topics.Where(c => c.Slug == slug).Count() > 0)
                {
                    return BadRequest("Tên danh mục đã tồn tại trong TOPIC, vui lòng thử lại!");
                }
                if (_context.Posts.Where(c => c.Slug == slug).Count() > 0)
                {
                    return BadRequest("Tên danh mục đã tồn tại trong POST, vui lòng thử lại!");
                }
                if (_context.Products.Where(c => c.Slug == slug).Count() > 0)
                {
                    return BadRequest("Tên danh mục đã tồn tại trong PRODUCT, vui lòng thử lại!");
                }
                topic.Slug= slug;
                topic.CreatedAt = DateTime.Now;
                topic.UpdatedAt = DateTime.Now;

                _context.Topics.Add(topic);
                await _context.SaveChangesAsync();
                return Ok("Added");
            }
            return BadRequest();
        }

        [HttpPut("id")]
        public async Task<IActionResult> EditTopic(int ID, Topic topic)
        {
            if (ModelState.IsValid)
            {
                if (topic.ParentId == null)
                {
                    topic.ParentId = 0;
                }
                String Slug = XString.ToAscii(topic.Name);
                if (_context.Categories.Where(m => m.Slug == Slug && m.Id != ID).Count() > 0)
                {
                    return BadRequest("Tên danh mục đã tồn tại, vui lòng thử lại!");
                }
                if (_context.Topics.Where(m => m.Slug == Slug && m.Id != ID).Count() > 0)
                {
                    return BadRequest("Tên danh mục đã tồn tại trong TOPIC, vui lòng thử lại!");
                }
                if (_context.Posts.Where(m => m.Slug == Slug && m.Id != ID).Count() > 0)
                {
                    return BadRequest("Tên danh mục đã tồn tại trong POST, vui lòng thử lại!");
                }
                if (_context.Products.Where(m => m.Slug == Slug && m.Id != ID).Count() > 0)
                {
                    return BadRequest("Tên danh mục đã tồn tại trong PRODUCT, vui lòng thử lại!");
                }

                topic.Slug = Slug;

                topic.CreatedAt = (from t in _context.Topics where t.Id == ID select t.CreatedAt).FirstOrDefault();
                topic.CreatedBy = (from t in _context.Topics where t.Id == ID select t.CreatedBy).FirstOrDefault();

                topic.UpdatedAt = DateTime.Now;

                _context.Entry(topic).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok("Edit Successfully");
            }
            return BadRequest();
        }
    }
}
