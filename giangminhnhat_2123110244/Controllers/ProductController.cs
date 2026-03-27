using EBikeAPI.Data;
using EBikeAPI.Models; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace giangminhnhat_2123110244.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
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
                return BadRequest("ID trên URL và ID trong thân Request không khớp nhau.");
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

        // 5. DELETE: Xóa 1 hoặc nhiều xe cùng lúc bằng chuỗi (DELETE)
        // Cú pháp gọi API trên Swagger: /api/Products/1,2,3
        [HttpDelete("{ids}")]
        public async Task<IActionResult> DeleteMultipleProducts(string ids)
        {
            if (string.IsNullOrWhiteSpace(ids))
            {
                return BadRequest("Vui lòng cung cấp danh sách ID.");
            }

            // Bước 1: Tách chuỗi "1,2,3" thành mảng số nguyên [1, 2, 3]
            var idList = ids.Split(',')
                            .Select(id => id.Trim())
                            .Where(id => int.TryParse(id, out _)) // Bỏ qua các ký tự không phải số
                            .Select(int.Parse)
                            .ToList();

            if (!idList.Any())
            {
                return BadRequest("Không tìm thấy ID nào hợp lệ để xóa.");
            }

            // Bước 2: Tìm tất cả các xe có ID nằm trong mảng vừa tách
            var productsToDelete = await _context.Products
                                                 .Where(p => idList.Contains(p.ProductId))
                                                 .ToListAsync();

            if (!productsToDelete.Any())
            {
                return NotFound("Không tìm thấy sản phẩm nào khớp với các ID đã cung cấp.");
            }

            // Bước 3: Dùng RemoveRange để xóa hàng loạt thay vì lặp vòng for (tối ưu hiệu năng)
            _context.Products.RemoveRange(productsToDelete);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Đã xóa thành công {productsToDelete.Count} sản phẩm." });
        }

        // Hàm hỗ trợ kiểm tra xe có tồn tại không
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}