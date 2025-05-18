using GmailAppApi.Data;
using GmailAppApi.Models;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            // Kiểm tra IMAP và SMTP song song để tăng tốc
            var imapTask = CheckImapAsync(req.Email, req.AppPassword);
            var smtpTask = CheckSmtpAsync(req.Email, req.AppPassword);

            await Task.WhenAll(imapTask, smtpTask);

            if (!imapTask.Result)
                return BadRequest(new { message = "❌ Đăng nhập IMAP thất bại" });

            if (!smtpTask.Result)
                return BadRequest(new { message = "❌ Đăng nhập SMTP thất bại" });

            var exist = await _dbContext.UserAccounts.FirstOrDefaultAsync(u => u.Username == req.Email);
            if (exist == null)
            {
                var account = new UserAccount
                {
                    Username = req.Email,
                    Password = req.AppPassword,
                    LoginDate = System.DateTime.Now
                };
                _dbContext.UserAccounts.Add(account);
                await _dbContext.SaveChangesAsync();
            }

            return Ok(new { message = "✅ Đăng nhập thành công!" });
        }

        private async Task<bool> CheckImapAsync(string email, string password)
        {
            try
            {
                using var client = new ImapClient();
                await client.ConnectAsync("imap.gmail.com", 993, true);
                await client.AuthenticateAsync(email, password);
                await client.DisconnectAsync(true);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> CheckSmtpAsync(string email, string password)
        {
            try
            {
                using var client = new SmtpClient();
                await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(email, password);
                await client.DisconnectAsync(true);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
