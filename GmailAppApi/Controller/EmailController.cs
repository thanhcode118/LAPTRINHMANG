using GmailAppApi.Models;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Search;
using MailKit;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System.Collections.Generic;
using System.Linq;

namespace GmailAppApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        [HttpPost("read")]
        public ActionResult<List<object>> ReadEmails([FromBody] EmailRequest request)
        {
            var emails = new List<object>();

            try
            {
                using var client = new ImapClient();
                client.Connect("imap.gmail.com", 993, true);
                client.Authenticate(request.Email, request.AppPassword);

                var inbox = client.GetFolder("INBOX");
                inbox.Open(FolderAccess.ReadOnly);

                var uids = inbox.Search(SearchQuery.NotDeleted);
                var latestUids = uids.OrderByDescending(uid => uid.Id).Take(10).ToList();

                foreach (var uid in latestUids)
                {
                    var message = inbox.GetMessage(uid);
                    emails.Add(new
                    {
                        From = message.From.ToString(),
                        Subject = message.Subject,
                        Body = message.TextBody ?? "(Không có nội dung)"
                    });
                }

                client.Disconnect(true);
            }
            catch
            {
                return BadRequest("Đọc email thất bại. Vui lòng kiểm tra lại thông tin.");
            }

            return Ok(emails);
        }

        [HttpPost("send")]
        public ActionResult SendEmail([FromBody] SendEmailRequest request)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(MailboxAddress.Parse(request.FromEmail));
                message.To.Add(MailboxAddress.Parse(request.ToEmail));
                message.Subject = request.Subject;
                message.Body = new TextPart("plain") { Text = request.Body };

                using var smtp = new SmtpClient();
                smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate(request.FromEmail, request.AppPassword);
                smtp.Send(message);
                smtp.Disconnect(true);

                return Ok("Gửi email thành công!");
            }
            catch
            {
                return BadRequest("Gửi email thất bại. Vui lòng kiểm tra lại thông tin.");
            }
        }
    }
}
