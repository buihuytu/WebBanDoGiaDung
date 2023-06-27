using DoGiaDung.Library;
using DoGiaDung.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DoGiaDung.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly WebbangiadungContext _context;
        public UsersController(WebbangiadungContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var countTrash = await _context.Users.Where(m => m.Status == 0).CountAsync();
            var list = await _context.Users.Where(m => m.Status != 0).ToListAsync();
            return Ok(new {countTrash, list});
        }

        [HttpGet]
        [Route("Trash")]
        public async Task<IActionResult> GetTrash()
        {
            var list = await _context.Users.Where(m => m.Status == 0).ToListAsync();
            return Ok(list);
        }

        [HttpDelete("id")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return BadRequest();
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok("Deleted");
        }

        [HttpPost]
        public async Task<IActionResult> InsertUser([FromForm] UserImage u)
        {
            if(ModelState.IsValid)
            {
                String avatar = XString.ToAscii(u.Fullname);
                var user = new User
                {
                    Fullname = u.Fullname,
                    Name = u.Name,
                    Password = XString.ToMD5(u.Password),
                    Email = u.Email,
                    Gender = u.Gender,
                    Phone = u.Phone,
                    Address = u.Address,
                    Access = u.Access,
                    Status = u.Status,
                    CreatedAt = DateTime.Now,
                    CreatedBy = u.CreatedBy,
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = u.UpdatedBy,
                };
                if(u.FileImage!= null)
                {
                    String fileName = avatar + u.FileImage.FileName.Substring(u.FileImage.FileName.LastIndexOf('.'));
                    var path = Path.Combine("D:\\Download\\Đồ án\\Code\\ShopGiaDung\\WebsiteBanDoGiaDung\\Public\\images\\user", fileName);
                    using (var stream = System.IO.File.Create(path))
                    {
                        await u.FileImage.CopyToAsync(stream);
                    }
                    user.Image = fileName;
                }
                else
                {
                    user.Image = null;
                }
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return Ok("Add successfully");
            }
            return BadRequest();
        }

        [HttpPut("id")]
        public async Task<IActionResult> EditUser(int ID, [FromForm] UserImage u)
        {
            if(ModelState.IsValid)
            {
                String avatar = XString.ToAscii(u.Fullname);
                var user = new User
                {
                    Id = ID,
                    Fullname = u.Fullname,
                    Name = u.Name,
                    Password = XString.ToMD5(u.Password),
                    Email = u.Email,
                    Gender = u.Gender,
                    Phone = u.Phone,
                    Address = u.Address,
                    Access = u.Access,
                    Status = u.Status,
                    CreatedAt = (from c in _context.Users where c.Id == ID select c.CreatedAt).FirstOrDefault(),
                    CreatedBy = (from c in _context.Users where c.Id == ID select c.CreatedBy).FirstOrDefault(),
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = u.UpdatedBy,
                };
                if (u.FileImage != null)
                {
                    String fileName = avatar + u.FileImage.FileName.Substring(u.FileImage.FileName.LastIndexOf('.'));
                    var path = Path.Combine("D:\\Download\\Đồ án\\Code\\ShopGiaDung\\WebsiteBanDoGiaDung\\Public\\images\\user", fileName);
                    using (var stream = System.IO.File.Create(path))
                    {
                        await u.FileImage.CopyToAsync(stream);
                    }
                    user.Image = fileName;
                }
                else
                {
                    user.Image = (from c in _context.Users where c.Id == ID select c.Image).FirstOrDefault();
                }
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok("Edit successfully");
            }
            return BadRequest();
        }
    }
}
