using GmailAppApi.Data;
using GmailAppApi.Models;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;

namespace GmailAppApi.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public AuthController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest req)
        {
            if (!CheckImap(req.Email, req.AppPassword))
                return BadRequest(new { message = "❌ Đăng nhập IMAP thất bại" });

            if (!CheckSmtp(req.Email, req.AppPassword))
                return BadRequest(new { message = "❌ Đăng nhập SMTP thất bại" });

            // Lưu vào database
            var exist = _dbContext.UserAccounts.FirstOrDefault(u => u.Username == req.Email);
            if (exist == null)
            {
                var account = new UserAccount
                {
                    Username = req.Email,
                    Password = req.AppPassword,
                    LoginDate = DateTime.Now
                };
                _dbContext.UserAccounts.Add(account);
                _dbContext.SaveChanges();
            }

            return Ok(new { message = "✅ Đăng nhập thành công!" });
        }

        private bool CheckImap(string email, string password)
        {
            try
            {
                using var client = new ImapClient();
                client.Connect("imap.gmail.com", 993, true);
                client.Authenticate(email, password);
                client.Disconnect(true);
                return true;
            }
            catch { return false; }
        }

        private bool CheckSmtp(string email, string password)
        {
            try
            {
                using var client = new SmtpClient();
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                client.Authenticate(email, password);
                client.Disconnect(true);
                return true;
            }
            catch { return false; }
        }
    }
}
