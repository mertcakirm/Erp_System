using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Erp_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private static List<TenantObj> _tenants = new();
        private static List<UserObj> _users = new();

        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterTenantRequest request)
        {
            if (_tenants.Any(t => t.Name.ToLower() == request.TenantName.ToLower()))
                return BadRequest("Bu tenant adı zaten kayıtlı.");

            var nextTenantId = _tenants.Any() ? _tenants.Max(t => t.Id) + 1 : 1;

            var tenant = new TenantObj
            {
                Id = nextTenantId,
                Name = request.TenantName,
                CreatedAt = DateTime.UtcNow
            };

            var hashedPassword = HashPassword(request.Password);

            var nextUserId = _users.Any() ? _users.Max(u => u.Id) + 1 : 1;

            var user = new UserObj
            {
                Id = nextUserId,
                Email = request.Email,
                PasswordHash = hashedPassword,
                TenantId = tenant.Id
            };

            _tenants.Add(tenant);
            _users.Add(user);

            return Ok("Tenant ve kullanıcı başarıyla oluşturuldu.");
        }

        [HttpPost("login/step1")]
        public IActionResult LoginStep1([FromBody] TenantCheckRequest request)
        {
            var tenant = _tenants.FirstOrDefault(t => t.Name.ToLower() == request.TenantName.ToLower());
            if (tenant == null)
                return NotFound("Tenant bulunamadı.");

            return Ok(new { TenantId = tenant.Id });
        }

        [HttpPost("login/step2")]
        public IActionResult LoginStep2([FromBody] LoginRequest request)
        {
            var user = _users.FirstOrDefault(u =>
                u.Email == request.Email &&
                u.TenantId == request.TenantId
            );

            if (user == null)
                return Unauthorized("Kullanıcı bulunamadı.");

            var hashedInput = HashPassword(request.Password);
            if (user.PasswordHash != hashedInput)
                return Unauthorized("Hatalı şifre.");

            var token = GenerateJwtToken(user);

            return Ok(new { Token = token, UserId = user.Id, TenantId = user.TenantId });
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hashBytes = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hashBytes);
        }

        private string GenerateJwtToken(UserObj user)
        {
            var tenant = _tenants.First(t => t.Id == user.TenantId);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("tenant_id", tenant.Id.ToString()),
                new Claim("tenant_name", tenant.Name)
            };
            
            var secretKey = _configuration["JwtSettings:Key"];
            if (string.IsNullOrEmpty(secretKey))
                throw new Exception("JWT Secret Key appsettings.json içinde tanımlanmadı.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class TenantObj
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }

    public class UserObj
    {
        public long Id { get; set; }
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public long TenantId { get; set; }
    }

    public class RegisterTenantRequest
    {
        public string TenantName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class TenantCheckRequest
    {
        public string TenantName { get; set; } = null!;
    }

    public class LoginRequest
    {
        public long TenantId { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}