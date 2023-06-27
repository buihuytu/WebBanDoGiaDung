using DoGiaDung.Library;
using DoGiaDung.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;

namespace DoGiaDung.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly WebbangiadungContext _context;
        public PostsController(WebbangiadungContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPosts()
        {
            // query to join post and topic
            var countTrash = _context.Posts.Where(m => m.Status == 0 && m.Type == "post").Count();
            var list = from p in _context.Posts
                       join t in _context.Topics
                       on p.TopicId equals t.Id
                       where p.Status != 0
                       orderby p.CreatedAt descending
                       select new PostTopic()
                       {
                           PostId = p.Id,
                           PostImg = p.Image,
                           PostName = p.Title,
                           PostStatus = p.Status,
                           TopicName = t.Name
                       };
            await list.ToListAsync();
            return Ok(new { countTrash, list });
        }

        [HttpGet]
        [Route("Trash")]
        public async Task<IActionResult> GetTrash()
        {
            var list = from p in _context.Posts
                       join t in _context.Topics
                       on p.TopicId equals t.Id
                       where p.Status == 0
                       orderby p.CreatedAt descending
                       select new PostTopic()
                       {
                           PostId = p.Id,
                           PostImg = p.Image,
                           PostName = p.Title,
                           PostStatus = p.Status,
                           TopicName = t.Name
                       };
            return Ok(await list.ToListAsync());
        }

        [HttpDelete("id")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return BadRequest();
            }
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return Ok("Deleted");
        }

        [HttpPost]
        public async Task<IActionResult> InsertPost([FromForm] PostImage p)
        {
            if(ModelState.IsValid)
            {
                String strSlug = XString.ToAscii(p.Title);
                var post = new Post
                {
                    TopicId = p.TopicId,
                    Slug = strSlug,
                    Title = p.Title,
                    Description = p.Description,
                    Detail = p.Detail,
                    Type = "post",
                    Position = p.Position,
                    MetaKey = p.MetaKey,
                    MetaDesc = p.MetaDesc,
                    Status = p.Status,
                    CreatedAt = DateTime.Now,
                    CreatedBy = p.CreatedBy,
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = p.UpdatedBy,
                };
                if (p.FileImage != null)
                {
                    String fileName = strSlug + p.FileImage.FileName.Substring(p.FileImage.FileName.LastIndexOf('.'));
                    var path = Path.Combine("D:\\Download\\Đồ án\\Code\\ShopGiaDung\\WebsiteBanDoGiaDung\\Public\\images\\post", fileName);
                    using (var stream = System.IO.File.Create(path))
                    {
                        await p.FileImage.CopyToAsync(stream);
                    }
                    post.Image = fileName;
                }
                else
                {
                    post.Image = null;
                }
                _context.Posts.Add(post);
                await _context.SaveChangesAsync();
                return Ok("Add successfully");
            }
            return BadRequest();
        }

        [HttpPut("id")]
        public async Task<IActionResult> EditPost(int ID, [FromForm] PostImage p)
        {
            if(ModelState.IsValid)
            {
                String strSlug = XString.ToAscii(p.Title);
                var post = new Post
                {
                    Id = ID,
                    TopicId = p.TopicId,
                    Slug = strSlug,
                    Title = p.Title,
                    Description = p.Description,
                    Detail = p.Detail,
                    Type = "post",
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
                    var path = Path.Combine("D:\\Download\\Đồ án\\Code\\ShopGiaDung\\WebsiteBanDoGiaDung\\Public\\images\\post", fileName);
                    using (var stream = System.IO.File.Create(path))
                    {
                        await p.FileImage.CopyToAsync(stream);
                    }
                    post.Image = fileName;
                }
                else
                {
                    post.Image = (from c in _context.Posts where c.Id == ID select c.Image).FirstOrDefault(); ;
                }
                _context.Entry(post).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok("Edit successfully");
            }
            return BadRequest();
        }
    }
}
