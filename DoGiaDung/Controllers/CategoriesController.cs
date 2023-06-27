using DoGiaDung.Library;
using DoGiaDung.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DoGiaDung.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly WebbangiadungContext _context;
        public CategoriesController(WebbangiadungContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var countTrash = await _context.Categories.Where(m => m.Status == 0).CountAsync();
            var list = await _context.Categories.Where(m => m.Status != 0).ToListAsync();
            foreach (var row in list)
            {
                var temp_link = _context.Links.Where(m => m.Type == "category" && m.TableId == row.Id);
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
                    row_link.Type = "category";
                    row_link.TableId = row.Id;
                    _context.Links.Add(row_link);
                }
            }
            _context.SaveChanges();
            return Ok(new { countTrash,list });
        }

        [HttpGet]
        [Route("Trash")]
        public async Task<IActionResult> GetTrash()
        {
            var list = await _context.Categories.Where(m => m.Status == 0).ToListAsync();
            return Ok(list);
        }

        [HttpGet("id")]
        public async Task<IActionResult> Detail(int? id)
        {
            if(id == null)
            {
                return BadRequest();
            }
            Category? category = await _context.Categories.Where(x => x.Id == id).FirstOrDefaultAsync();
            if(category == null)
            {
                return BadRequest();
            }
            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> InsertCategory(string? name, int? parentId, int orders, string? metakey, string? metadesc, int status, int createdBy, int updatedBy)
        {
            if(parentId == null)
            {
                parentId = 0;
            }
            
            String slug = XString.ToAscii(name);
            CheckSlug check = new CheckSlug(_context);

            if(!check.KiemTraSlug("Category", slug, null))
            {
                return BadRequest();
            }
            
            

            Category newCategory = new Category();
            newCategory.Name = name;
            newCategory.Slug = slug;
            newCategory.ParentId = parentId;
            newCategory.Orders = orders;
            newCategory.MetaKey= metakey;
            newCategory.MetaDesc = metadesc;
            newCategory.Status = status;
            newCategory.CreatedAt = DateTime.Now;
            newCategory.CreatedBy = createdBy;
            newCategory.UpdatedAt = DateTime.Now;
            newCategory.UpdatedBy = updatedBy;

            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();
            return Ok("Added");
        }

        [HttpDelete("id")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if(category == null)
            {
                return BadRequest();
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return Ok("Deleted");
        }

        [HttpPut("id")]
        public async Task<IActionResult> EditCategory(int ID, Category category)
        {
            String Slug = XString.ToAscii(category.Name);
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

            category.Slug = Slug;
            category.CreatedAt = (from c in _context.Categories where c.Id == ID select c.CreatedAt).FirstOrDefault();
            category.CreatedBy = (from c in _context.Categories where c.Id == ID select c.CreatedBy).FirstOrDefault();
            category.UpdatedAt = DateTime.Now;

            _context.Entry(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok("Edit Successfully");
        }
    }
}
