using EBikeAPI.Data;
using EBikeAPI.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace giangminhnhat_2123110244.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InventoriesController(AppDbContext context) { _context = context; }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Inventory>>> GetInventories() { return await _context.Inventories.Include(i => i.Product).ToListAsync(); }

        [HttpGet("{id}")]
        public async Task<ActionResult<Inventory>> GetInventory(int id)
        {
            var inventory = await _context.Inventories.FindAsync(id);
            return inventory == null ? NotFound() : inventory;
        }

        [HttpPost]
        public async Task<ActionResult<Inventory>> PostInventory(Inventory inventory)
        {
            _context.Inventories.Add(inventory);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetInventory", new { id = inventory.InventoryId }, inventory);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutInventory(int id, Inventory inventory)
        {
            if (id != inventory.InventoryId) return BadRequest("Sai ID");

            // Lấy dữ liệu cũ từ DB lên, kèm theo thông tin Product
            var existingInventory = await _context.Inventories
                                                  .Include(i => i.Product)
                                                  .FirstOrDefaultAsync(i => i.InventoryId == id);

            if (existingInventory == null) return NotFound();

            // 1. Cập nhật thông tin bảng KHO (Inventory)
            existingInventory.SerialNumber = inventory.SerialNumber;
            existingInventory.Status = inventory.Status;

            // 2. Cập nhật thông tin bảng XE (Product)
            if (inventory.Product != null && existingInventory.Product != null)
            {
                existingInventory.Product.Model = inventory.Product.Model;
                existingInventory.Product.Color = inventory.Product.Color;
                existingInventory.Product.Price = inventory.Product.Price;

                // Nếu có upload ảnh mới thì mới cập nhật URL ảnh
                if (!string.IsNullOrEmpty(inventory.Product.ImageUrl))
                {
                    existingInventory.Product.ImageUrl = inventory.Product.ImageUrl;
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Trả về lỗi chi tiết nếu còn lỗi 500 để dễ sửa
                return StatusCode(500, $"Lỗi Server: {ex.Message}");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventory(int id)
        {
            var inventory = await _context.Inventories.FindAsync(id);
            if (inventory == null) return NotFound();
            _context.Inventories.Remove(inventory);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}