using DoGiaDung.Library;
using DoGiaDung.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DoGiaDung.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SlidersController : ControllerBase
    {
        private readonly WebbangiadungContext _context;
        public SlidersController(WebbangiadungContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSliders()
        {
            var countTrash = _context.Sliders.Where(m => m.Status == 0).Count();
            var list = await _context.Sliders.Where(m => m.Status != 0).ToListAsync();
            foreach(var item in list)
            {
                _context.Entry(item).State = EntityState.Modified;
            }
            return Ok(new { countTrash, list });
        }

        [HttpGet]
        [Route("Trash")]
        public async Task<IActionResult> GetTrash()
        {
            var list = await _context.Sliders.Where(m => m.Status == 0).ToListAsync();
            return Ok(list);
        }

        [HttpDelete("id")]
        public async Task<IActionResult> DeleteSlider(int id)
        {
            var slider = await _context.Sliders.FindAsync(id);
            if (slider == null)
            {
                return BadRequest();
            }
            _context.Sliders.Remove(slider);
            await _context.SaveChangesAsync();
            return Ok("Deleted");
        }

        [HttpPost]
        public async Task<IActionResult> InsertSlider([FromForm] SliderImage s)
        {
            if (ModelState.IsValid)
            {
                String strSlug = XString.ToAscii(s.Name);
                var slider = new Slider
                {
                    Name = s.Name,
                    Url = strSlug,
                    Position = s.Position,
                    Orders = 0,
                    Status = s.Status,
                    CreatedAt = DateTime.Now,
                    CreatedBy = s.CreatedBy,
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = s.UpdatedBy,
                };
                if (s.FileImage != null)
                {
                    String fileName = strSlug + s.FileImage.FileName.Substring(s.FileImage.FileName.LastIndexOf("."));
                    var path = Path.Combine("D:\\Download\\Đồ án\\Code\\ShopGiaDung\\WebsiteBanDoGiaDung\\Public\\images\\sliders", fileName);
                    using (var stream = System.IO.File.Create(path))
                    {
                        await s.FileImage.CopyToAsync(stream);
                    }
                    slider.Image = fileName;
                }
                else
                {
                    slider.Image = "";
                }
                _context.Sliders.Add(slider);
                await _context.SaveChangesAsync();
                return Ok("Add successfully");
            }
            return BadRequest();
        }

        [HttpPut("id")]
        public async Task<IActionResult> UpdateSlider(int ID, [FromForm] SliderImage s)
        {
            if(ModelState.IsValid)
            {
                String strSlug = XString.ToAscii(s.Name);
                var slider = new Slider
                {
                    Id = ID,
                    Name = s.Name,
                    Url = strSlug,
                    Position = s.Position,
                    Orders = 0,
                    Status = s.Status,
                    CreatedAt = (from c in _context.Sliders where c.Id == ID select c.CreatedAt).FirstOrDefault(),
                    CreatedBy = (from c in _context.Sliders where c.Id == ID select c.CreatedBy).FirstOrDefault(),
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = s.UpdatedBy,
                };
                if (s.FileImage != null)
                {
                    String fileName = strSlug + s.FileImage.FileName.Substring(s.FileImage.FileName.LastIndexOf("."));
                    var path = Path.Combine("D:\\Download\\Đồ án\\Code\\ShopGiaDung\\WebsiteBanDoGiaDung\\Public\\images\\sliders", fileName);
                    using (var stream = System.IO.File.Create(path))
                    {
                        await s.FileImage.CopyToAsync(stream);
                    }
                    slider.Image = fileName;
                }
                else
                {
                    slider.Image = (from c in _context.Sliders where c.Id == ID select c.Image).FirstOrDefault();
                }
                _context.Entry(slider).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok("Edit Successfully");
            }
            return BadRequest();
        }
    }
}
