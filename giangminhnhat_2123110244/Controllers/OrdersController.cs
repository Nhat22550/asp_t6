using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
// NẾU BỊ GẠCH ĐỎ 2 DÒNG DƯỚI, HÃY ĐỔI 'EBikeAPI' THÀNH 'giangminhnhat_2123110244' NHÉ
using EBikeAPI.Data;
using EBikeAPI.Models;

namespace EBikeAPI.Controllers // NẾU BỊ LỖI NAMESPACE THÌ ĐỔI GIỐNG NHƯ TRÊN
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        // 1. HÀM CỦA BẠN: Lấy danh sách đơn hàng (GET)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Select(o => new {
                    o.OrderId,
                    OrderDate = o.CreatedDate, // Đã fix chuẩn
                    o.TotalAmount,
                    // Status = o.Status, // Mở comment dòng này nếu bảng Order có cột Status
                    CustomerName = o.Customer != null ? o.Customer.Name : "Khach vang lai",
                    Items = new List<string>()
                })
                .ToListAsync();

            return Ok(orders);
        }

        // 2. HÀM BỔ SUNG: Nhận đơn đặt hàng từ React (POST) - GIẢI QUYẾT LỖI 405
        [HttpPost]
        public async Task<IActionResult> CreateOrder(Order order)
        {
            if (order == null)
            {
                return BadRequest("Dữ liệu đơn hàng không hợp lệ.");
            }

            // Gán ngày tạo đơn hàng là thời điểm hiện tại (Vì model của bạn dùng CreatedDate)
            order.CreatedDate = DateTime.Now;

            // Thêm vào DB
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Thanh toán thành công!", orderId = order.OrderId });
        }
    }
}