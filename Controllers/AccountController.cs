using Microsoft.AspNetCore.Mvc;
using EmployeeMVC.Data;
using EmployeeMVC.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace EmployeeMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public AccountController(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        // REGISTER
        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(User user)
        {
            user.PasswordHash = Hash(user.Password);
            user.Role = "HR"; // Only HR can login
            _db.Userss.Add(user);
            _db.SaveChanges();
            return RedirectToAction("Login");
        }

        // LOGIN
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {
            string hash = Hash(user.Password);

            var dbUser = _db.Userss.FirstOrDefault(
                x => x.Email == user.Email && x.PasswordHash == hash);

            if (dbUser == null)
                return View();

            // 🔐 Generate JWT
            string token = GenerateToken(dbUser.Email);

            Response.Cookies.Append("JWToken", token, new CookieOptions
            {
                Expires = DateTime.Now.AddHours(1),
                HttpOnly = false,
                IsEssential = true
            });

            // 🍪 Cookie login
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, dbUser.Email)
            };

            var identity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            return RedirectToAction("Index", "Employee");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            Response.Cookies.Delete("JWToken");
            return RedirectToAction("Login");
        }

        // PASSWORD HASH
        private string Hash(string password)
        {
            var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        // JWT TOKEN
        private string GenerateToken(string email)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[] { new Claim(ClaimTypes.Email, email) };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}