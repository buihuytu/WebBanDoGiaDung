using DoGiaDung.Library;
using DoGiaDung.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DoGiaDung.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagesController : ControllerBase
    {
        private readonly WebbangiadungContext _context;
        public PagesController(WebbangiadungContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPages()
        {
            var countTrash = await _context.Posts.Where(m => m.Status == 0 && m.Type == "page").CountAsync();
            var list = await _context.Posts.Where(m => m.Status != 0 && m.Type == "page").ToListAsync();
            foreach (var row in list)
            {
                var temp_link = _context.Links.Where(m => m.Type == "page" && m.TableId == row.Id);
                if (temp_link.Count() > 0)
                {
                    var row_link = temp_link.First();
                    row_link.Name = row.Title;
                    row_link.Slug = row.Slug;
                    _context.Entry(row_link).State = EntityState.Modified;
                }
                else
                {
                    var row_link = new Link();
                    row_link.Name = row.Title;
                    row_link.Slug = row.Slug;
                    row_link.Type = "page";
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
            var list = await _context.Posts.Where(m => m.Status == 0 && m.Type == "page").ToListAsync();
            return Ok(list);
        }

        [HttpDelete("id")]
        public async Task<IActionResult> DeletePage(int id)
        {
            var page = await _context.Posts.FindAsync(id);
            if (page == null)
            {
                return BadRequest();
            }
            _context.Posts.Remove(page);
            await _context.SaveChangesAsync();
            return Ok("Deleted");
        }

        [HttpPost]
        public async Task<IActionResult> InsertPage([FromForm] PostImage p)
        {
            if(ModelState.IsValid)
            {
                String strSlug = XString.ToAscii(p.Title);
                var page = new Post
                {
                    TopicId = p.TopicId,
                    Slug = strSlug,
                    Title = p.Title,
                    Description = p.Description,
                    Detail = p.Detail,
                    Type = "page",
                    Position = p.Position,
                    MetaKey = p.MetaKey,
                    MetaDesc = p.MetaDesc,
                    Status = p.Status,
                    CreatedAt = DateTime.Now,
                    CreatedBy = p.CreatedBy,
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = p.UpdatedBy,
                };
                if(p.FileImage != null)
                {
                    String fileName = strSlug + p.FileImage.FileName.Substring(p.FileImage.FileName.LastIndexOf('.'));
                    var path = Path.Combine("D:\\Download\\Đồ án\\Code\\ShopGiaDung\\WebsiteBanDoGiaDung\\Public\\images\\page", fileName);
                    using (var stream = System.IO.File.Create(path))
                    {
                        await p.FileImage.CopyToAsync(stream);
                    }
                    page.Image = fileName;
                }
                else
                {
                    page.Image = null;
                }
                _context.Posts.Add(page);
                await _context.SaveChangesAsync();
                return Ok("Add successfully");
            }
            return BadRequest();
        }

        [HttpPut("id")]
        public async Task<IActionResult> EditPage(int ID, [FromForm] PostImage p)
        {
            if (ModelState.IsValid)
            {
                String strSlug = XString.ToAscii(p.Title);
                var page = new Post
                {
                    Id = ID,
                    TopicId = p.TopicId,
                    Slug = strSlug,
                    Title = p.Title,
                    Description = p.Description,
                    Detail = p.Detail,
                    Type = "page",
                    Position = p.Position,
                    MetaKey = p.MetaKey,
                    MetaDesc = p.MetaDesc,
                    Status = p.Status,
                    CreatedAt = (from c in _context.Posts where c.Id == ID select c.CreatedAt).FirstOrDefault(),
                    CreatedBy = (from c in _context.Posts where c.Id == ID select c.CreatedBy).FirstOrDefault(),
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = p.UpdatedBy,
                };
                if (p.FileImage != null)
                {
                    String fileName = strSlug + p.FileImage.FileName.Substring(p.FileImage.FileName.LastIndexOf('.'));
                    var path = Path.Combine("D:\\Download\\Đồ án\\Code\\ShopGiaDung\\WebsiteBanDoGiaDung\\Public\\images\\page", fileName);
                    using (var stream = System.IO.File.Create(path))
                    {
                        await p.FileImage.CopyToAsync(stream);
                    }
                    page.Image = fileName;
                }
                else
                {
                    page.Image = (from c in _context.Posts where c.Id == ID select c.Image).FirstOrDefault();
                }
                _context.Entry(page).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok("Edit successfully");
            }
            return BadRequest();
        }
    }
}
