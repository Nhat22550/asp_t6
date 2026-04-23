using EBikeAPI.Data;
using EBikeAPI.Models; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace giangminhnhat_2123110244.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductsController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // 1. CREATE: Thêm mới 1 chiếc xe (POST)
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId }, product);
        }

        // 2. READ: Lấy danh sách tất cả xe (GET)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        // 3. READ: Lấy thông tin 1 chiếc xe theo ID (GET)
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // 4. UPDATE: Cập nhật thông tin xe (PUT)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.ProductId)
            {
                return BadRequest("ID in URL and ID in request body do not match.");
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // 5a. DELETE: Xóa mot san pham theo ID
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 5b. DELETE: Xoa nhieu san pham cung luc (duong dan: /api/Products/multiple/1,2,3)
        [HttpDelete("multiple/{ids}")]
        public async Task<IActionResult> DeleteMultipleProducts(string ids)
        {
            if (string.IsNullOrWhiteSpace(ids))
            {
                return BadRequest("Vui long cung cap danh sach ID.");
            }

            var idList = ids.Split(',')
                            .Select(i => i.Trim())
                            .Where(i => int.TryParse(i, out _))
                            .Select(int.Parse)
                            .ToList();

            if (!idList.Any()) return BadRequest("Khong co ID hop le.");

            var productsToDelete = await _context.Products
                                                 .Where(p => idList.Contains(p.ProductId))
                                                 .ToListAsync();

            if (!productsToDelete.Any()) return NotFound("Khong tim thay san pham phu hop.");

            _context.Products.RemoveRange(productsToDelete);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Da xoa thanh cong {productsToDelete.Count} san pham." });
        }

        // Hàm hỗ trợ kiểm tra xe có tồn tại không
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }

        // Upload image for product
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest(new { message = "No file provided." });

            var uploadsRoot = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "products");
            if (!Directory.Exists(uploadsRoot)) Directory.CreateDirectory(uploadsRoot);

            var ext = Path.GetExtension(file.FileName);
            var fileName = Guid.NewGuid().ToString() + ext;
            var filePath = Path.Combine(uploadsRoot, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var url = $"/uploads/products/{fileName}";
            return Ok(new { imageUrl = url });
        }
    }
}