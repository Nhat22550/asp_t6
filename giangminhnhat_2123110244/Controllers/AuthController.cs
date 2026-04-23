using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EBikeAPI.Data;
using EBikeAPI.Models;

namespace EBikeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // 1. API DANG KY
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // Kiem tra xem tai khoan da ton tai chua
            var exists = await _context.Users.AnyAsync(u => u.Username == request.Username);
            if (exists)
            {
                return BadRequest(new { message = "Tai khoan nay da ton tai!" });
            }

            var newUser = new User
            {
                Username = request.Username,
                Password = request.Password, // Trong do an thuc te nen ma hoa (Hash) mat khau
                FullName = request.FullName,
                Email = request.Email,
                Role = "User"
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Dang ky thanh cong!" });
        }

        // 2. API DANG NHAP
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Tim user co Username va Password trung khop
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username && u.Password == request.Password);

            if (user == null)
            {
                return Unauthorized(new { message = "Sai tai khoan hoac mat khau!" });
            }

            // Tao mot token don gian de React nhan dien da dang nhap
            var fakeToken = "ebike-auth-token-" + user.UserId;

            return Ok(new
            {
                token = fakeToken,
                user = new
                {
                    user.UserId,
                    user.Username,
                    user.FullName,
                    user.Role
                }
            });
        }
    }

    // Cac class ho tro nhan du lieu tu React gui len
    public class RegisterRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}