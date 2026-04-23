using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Net;
// DOI 'EBikeAPI' THANH 'giangminhnhat_2123110244' NEU BI LOI DO
using EBikeAPI.Data;
using EBikeAPI.Models;

namespace EBikeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        // Them IConfiguration vao Constructor de doc file appsettings.json
        public OrdersController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // 1. GET: Lay danh sach don hang
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Select(o => new {
                    o.OrderId,
                    OrderDate = o.CreatedDate,
                    o.TotalAmount,
                    CustomerName = o.Customer != null ? o.Customer.Name : "Khach vang lai",
                    Items = new List<string>()
                })
                .ToListAsync();

            return Ok(orders);
        }

        // 2. POST: Tao don hang moi (Luu vao DB)
        [HttpPost]
        public async Task<IActionResult> CreateOrder(Order order)
        {
            if (order == null) return BadRequest("Du lieu khong hop le.");
            order.CreatedDate = DateTime.Now;
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Luu don hang thanh cong!", orderId = order.OrderId });
        }

        // 3. POST: Tao Link thanh toan VNPAY (Giai quyet nut Thanh toan Online)
        [HttpPost("create-payment-url")]
        public async Task<IActionResult> CreatePaymentUrl([FromBody] Order order)
        {
            // Buoc 1: Luu don hang tam thoi vao DB de lay OrderId
            order.CreatedDate = DateTime.Now;
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Buoc 2: Lay cau hinh VNPAY tu appsettings.json
            string vnp_Returnurl = _configuration["VnPay:ReturnUrl"];
            string vnp_Url = _configuration["VnPay:BaseUrl"];
            string vnp_TmnCode = _configuration["VnPay:TmnCode"];
            string vnp_HashSecret = _configuration["VnPay:HashSecret"];

            // Buoc 3: Tao danh sach tham so gui sang VNPAY
            var vnpayData = new SortedList<string, string>(new VnPayCompare());
            vnpayData.Add("vnp_Version", "2.1.0");
            vnpayData.Add("vnp_Command", "pay");
            vnpayData.Add("vnp_TmnCode", vnp_TmnCode);
            vnpayData.Add("vnp_Amount", (order.TotalAmount * 100).ToString()); // VNPAY yeu cau nhan 100
            vnpayData.Add("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpayData.Add("vnp_CurrCode", "VND");
            vnpayData.Add("vnp_IpAddr", "127.0.0.1");
            vnpayData.Add("vnp_Locale", "vn");
            vnpayData.Add("vnp_OrderInfo", "Thanh toan don hang: " + order.OrderId);
            vnpayData.Add("vnp_OrderType", "other");
            vnpayData.Add("vnp_ReturnUrl", vnp_Returnurl);
            vnpayData.Add("vnp_TxnRef", order.OrderId.ToString());

            // Buoc 4: Tao chuoi Query de Bam ma (SecureHash)
            StringBuilder data = new StringBuilder();
            foreach (KeyValuePair<string, string> kv in vnpayData)
            {
                data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
            }
            string rawData = data.ToString().TrimEnd('&');

            // Buoc 5: Tao SecureHash bang HMACSHA512
            string vnp_SecureHash = HMACSHA512(vnp_HashSecret, rawData);
            string paymentUrl = vnp_Url + "?" + rawData + "&vnp_SecureHash=" + vnp_SecureHash;

            return Ok(new { url = paymentUrl });
        }

        // Ham tro giup ma hoa
        private string HMACSHA512(string key, string inputData)
        {
            var hash = new StringBuilder();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue) hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }

        // Lop ho tro sap xep tham so theo Alphabet
        public class VnPayCompare : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                if (x == y) return 0;
                if (x == null) return -1;
                if (y == null) return 1;
                return string.CompareOrdinal(x, y);
            }
        }
    }
}