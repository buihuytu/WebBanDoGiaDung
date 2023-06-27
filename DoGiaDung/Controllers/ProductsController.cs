using DoGiaDung.Library;
using DoGiaDung.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace DoGiaDung.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly WebbangiadungContext _context;
        public ProductsController(WebbangiadungContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            // query to join Product vs Category

            var countTrash = _context.Products.Where(m => m.Status == 0).Count();
            var list = from p in _context.Products
                       join c in _context.Categories
                       on p.CateId equals c.Id
                       where p.Status != 0
                       where p.CateId == c.Id
                       orderby p.CreatedAt descending
                       select new ProductCategory()
                       {
                           ProductId = p.Id,
                           ProductImg = p.Image,
                           ProductName = p.Name,
                           ProductStatus = p.Status,
                           ProductDiscount = p.Discount,
                           CategoryName = c.Name
                       };
            await list.ToListAsync();
            return Ok(new { countTrash, list });
        }

        [HttpGet]
        [Route("GetProductPage/{page}")]
        public async Task<IActionResult> GetAllProductsByPage(int page)
        {
            // query to join Product vs Category

            var countTrash = _context.Products.Where(m => m.Status == 0).Count();
            int countProduct = _context.Products.Where(m => m.Status != 0).Count();
            var list = (from p in _context.Products
                       join c in _context.Categories
                       on p.CateId equals c.Id
                       where p.Status != 0
                       where p.CateId == c.Id
                       orderby p.CreatedAt descending
                       select new ProductCategory()
                       {
                           ProductId = p.Id,
                           ProductImg = p.Image,
                           ProductName = p.Name,
                           ProductStatus = p.Status,
                           ProductDiscount = p.Discount,
                           CategoryName = c.Name
                       });
            list = list.Skip((page - 1) * 10).Take(10);
            await list.ToListAsync();
            return Ok(new { countTrash, countProduct, list });
        }

        [HttpGet]
        [Route("Trash")]
        public async Task<IActionResult> GetTrash()
        {
            var list = from p in _context.Products
                       join c in _context.Categories
                       on p.CateId equals c.Id
                       where p.Status == 0
                       where p.CateId == c.Id
                       orderby p.CreatedAt descending
                       select new ProductCategory()
                       {
                           ProductId = p.Id,
                           ProductImg = p.Image,
                           ProductName = p.Name,
                           ProductStatus = p.Status,
                           ProductDiscount = p.Discount,
                           CategoryName = c.Name
                       };
            return Ok(await list.ToListAsync());
        }

        [HttpGet]
        [Route("GetProductByName/{name}")]
        public async Task<IActionResult> GetProductByName(string name)
        {
            var listProduct = from p in _context.Products
                       join c in _context.Categories
                       on p.CateId equals c.Id
                       where p.Status != 0
                       where p.CateId == c.Id
                       orderby p.CreatedAt descending
                       select new ProductCategory()
                       {
                           ProductId = p.Id,
                           ProductImg = p.Image,
                           ProductName = p.Name,
                           ProductStatus = p.Status,
                           ProductDiscount = p.Discount,
                           CategoryName = c.Name
                       };
            await listProduct.ToListAsync();
            var list = await listProduct.Where(p => p.ProductName.Contains(name)).ToListAsync();
            return Ok(list);
        }

        [HttpDelete("id")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return BadRequest();
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Ok("Deleted");
        }

        [HttpPost]
        public async Task<IActionResult> InsertProduct([FromForm] ProductImage p)
        {
            if(ModelState.IsValid)
            {
                String strSlug = XString.ToAscii(p.Name);
                var product = new Product
                {
                    Name = p.Name,
                    Slug = strSlug,
                    CateId = p.CateId,
                    ImageList = null,
                    NewPromotion = p.NewPromotion,
                    Installment = p.Installment,
                    Discount = p.Discount,
                    Detail = p.Detail,
                    Description = p.Description,
                    Specification = p.Specification,
                    Price = p.Price + 500000,
                    Quantity = p.Quantity,
                    ProPrice = p.ProPrice + 500000,
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
                    var path = Path.Combine("D:\\Download\\Đồ án\\Code\\ShopGiaDung\\WebsiteBanDoGiaDung\\Public\\images\\products", fileName);
                    using (var stream = System.IO.File.Create(path))
                    {
                        await p.FileImage.CopyToAsync(stream);
                    }
                    product.Image = fileName;
                }
                else
                {
                    product.Image = null;
                }
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return Ok("Add successfully");
            }
            return BadRequest();
        }

        [HttpPut("id")]
        public async Task<IActionResult> EditProduct(int ID, [FromForm] ProductImage p)
        {
            if (ModelState.IsValid)
            {
                String strSlug = XString.ToAscii(p.Name);
                var product = new Product
                {
                    Id = ID,
                    Name = p.Name,
                    Slug = strSlug,
                    CateId = p.CateId,
                    ImageList = null,
                    NewPromotion = p.NewPromotion,
                    Installment = p.Installment,
                    Discount = p.Discount,
                    Detail = p.Detail,
                    Description = p.Description,
                    Specification = p.Specification,
                    Price = p.Price + 500000,
                    Quantity = p.Quantity,
                    ProPrice = p.ProPrice + 500000,
                    MetaKey = p.MetaKey,
                    MetaDesc = p.MetaDesc,
                    Status = p.Status,
                    CreatedAt = (from c in _context.Products where c.Id == ID select c.CreatedAt).FirstOrDefault(),
                    CreatedBy = (from c in _context.Products where c.Id == ID select c.CreatedBy).FirstOrDefault(),
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = p.UpdatedBy,
                };
                if (p.FileImage != null)
                {
                    String fileName = strSlug + p.FileImage.FileName.Substring(p.FileImage.FileName.LastIndexOf('.'));
                    var path = Path.Combine("D:\\Download\\Đồ án\\Code\\ShopGiaDung\\WebsiteBanDoGiaDung\\Public\\images\\products", fileName);
                    using (var stream = System.IO.File.Create(path))
                    {
                        await p.FileImage.CopyToAsync(stream);
                    }
                    product.Image = fileName;
                }
                else
                {
                    product.Image = (from c in _context.Products where c.Id == ID select c.Image).FirstOrDefault(); ;
                }
                _context.Entry(product).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok("Edit successfully");
            }
            return BadRequest();
        }
    }
    
}
