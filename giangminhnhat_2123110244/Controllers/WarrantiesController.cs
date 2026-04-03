using EBikeAPI.Data;
using EBikeAPI.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace giangminhnhat_2123110244.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarrantiesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WarrantiesController(AppDbContext context) { _context = context; }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Warranty>>> GetWarranties() { return await _context.Warranties.ToListAsync(); }

        [HttpGet("{id}")]
        public async Task<ActionResult<Warranty>> GetWarranty(int id)
        {
            var warranty = await _context.Warranties.FindAsync(id);
            return warranty == null ? NotFound() : warranty;
        }

        [HttpPost]
        public async Task<ActionResult<Warranty>> PostWarranty(Warranty warranty)
        {
            _context.Warranties.Add(warranty);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetWarranty", new { id = warranty.WarrantyId }, warranty);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutWarranty(int id, Warranty warranty)
        {
            if (id != warranty.WarrantyId) return BadRequest();
            _context.Entry(warranty).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWarranty(int id)
        {
            var warranty = await _context.Warranties.FindAsync(id);
            if (warranty == null) return NotFound();
            _context.Warranties.Remove(warranty);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}