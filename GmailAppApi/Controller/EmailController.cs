using GmailAppApi.Models;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System.Collections.Generic;
using System.Linq;
using System;

namespace GmailAppApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        // Đọc email trong Inbox
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
                        Body = message.TextBody ?? "(Không có nội dung)",
                        Date = message.Date.ToString("g")
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

        // Đọc email trong thư mục Sent Mail
        [HttpPost("read/sent")]
        public ActionResult<List<object>> ReadSentEmails([FromBody] EmailRequest request)
        {
            var emails = new List<object>();

            try
            {
                using var client = new ImapClient();
                client.Connect("imap.gmail.com", 993, true);
                client.Authenticate(request.Email, request.AppPassword);

                var root = client.GetFolder(client.PersonalNamespaces[0]);
                var gmailFolder = root.GetSubfolder("[Gmail]");
                var sentFolder = gmailFolder.GetSubfolder("Thư đã gửi");

                sentFolder.Open(FolderAccess.ReadOnly);

                var uids = sentFolder.Search(SearchQuery.NotDeleted);
                var latestUids = uids.OrderByDescending(uid => uid.Id).Take(10).ToList();

                foreach (var uid in latestUids)
                {
                    var message = sentFolder.GetMessage(uid);
                    emails.Add(new
                    {
                        To = message.To.ToString(),
                        Subject = message.Subject,
                        Body = message.TextBody ?? "(Không có nội dung)",
                        Date = message.Date.ToString("g")
                    });
                }

                client.Disconnect(true);
            }
            catch
            {
                return BadRequest("Đọc email thư đã gửi thất bại. Vui lòng kiểm tra lại thông tin.");
            }

            return Ok(emails);
        }
        [HttpPost("read/starred")]
        public ActionResult<List<object>> ReadStarredEmails([FromBody] EmailRequest request)
        {
            var emails = new List<object>();

            try
            {
                using var client = new ImapClient();
                client.Connect("imap.gmail.com", 993, true);
                client.Authenticate(request.Email, request.AppPassword);

                var root = client.GetFolder(client.PersonalNamespaces[0]);
                var gmailFolder = root.GetSubfolder("[Gmail]");
                var starredFolder = gmailFolder.GetSubfolder("Có gắn dấu sao"); // hoặc "Starred"

                starredFolder.Open(FolderAccess.ReadOnly);

                var uids = starredFolder.Search(SearchQuery.NotDeleted);
                var latestUids = uids.OrderByDescending(uid => uid.Id).Take(10).ToList();

                foreach (var uid in latestUids)
                {
                    var message = starredFolder.GetMessage(uid);
                    emails.Add(new
                    {
                        From = message.From.ToString(),
                        Subject = message.Subject,
                        Body = message.TextBody ?? "(Không có nội dung)",
                        Date = message.Date.ToString("g")
                    });
                }

                client.Disconnect(true);
            }
            catch (Exception ex)
            {
                return BadRequest($"Đọc email thất bại: {ex.Message}");
            }

            return Ok(emails);
        }

        [HttpPost("read/trash")]
        public ActionResult<List<object>> ReadTrashEmails([FromBody] EmailRequest request)
        {
            var emails = new List<object>();

            try
            {
                using var client = new ImapClient();
                client.Connect("imap.gmail.com", 993, true);
                client.Authenticate(request.Email, request.AppPassword);

                var root = client.GetFolder(client.PersonalNamespaces[0]);
                var gmailFolder = root.GetSubfolder("[Gmail]");

                // Thử các tên folder phổ biến của Trash:
                string[] possibleTrashNames = { "Thùng rác", "Rác", "Deleted Messages", "Trash" };
                IMailFolder trashFolder = null;

                foreach (var name in possibleTrashNames)
                {
                    try
                    {
                        trashFolder = gmailFolder.GetSubfolder(name);
                        if (trashFolder != null) break;
                    }
                    catch { }
                }

                if (trashFolder == null)
                    return BadRequest("Không tìm thấy thư mục Thùng rác.");

                trashFolder.Open(FolderAccess.ReadOnly);

                var uids = trashFolder.Search(SearchQuery.NotDeleted);
                var latestUids = uids.OrderByDescending(uid => uid.Id).Take(10).ToList();

                foreach (var uid in latestUids)
                {
                    var message = trashFolder.GetMessage(uid);
                    emails.Add(new
                    {
                        From = message.From.ToString(),
                        Subject = message.Subject,
                        Body = message.TextBody ?? "(Không có nội dung)",
                        Date = message.Date.ToString("g")
                    });
                }

                client.Disconnect(true);
            }
            catch (Exception ex)
            {
                return BadRequest($"Đọc email thất bại: {ex.Message}");
            }

            return Ok(emails);
        }


        [HttpPost("read/important")]
        public ActionResult<List<object>> ReadImportantEmails([FromBody] EmailRequest request)
        {
            var emails = new List<object>();

            try
            {
                using var client = new ImapClient();
                client.Connect("imap.gmail.com", 993, true);
                client.Authenticate(request.Email, request.AppPassword);

                var root = client.GetFolder(client.PersonalNamespaces[0]);
                var gmailFolder = root.GetSubfolder("[Gmail]");
                // Thư mục Important có thể tên "Quan trọng" hoặc "Important"
                var importantFolder = gmailFolder.GetSubfolders(false)
                                    .FirstOrDefault(f => f.Name.Equals("Quan trọng", StringComparison.OrdinalIgnoreCase)
                                                      || f.Name.Equals("Important", StringComparison.OrdinalIgnoreCase));

                if (importantFolder == null)
                    return BadRequest("Không tìm thấy thư mục Quan trọng.");

                importantFolder.Open(FolderAccess.ReadOnly);

                var uids = importantFolder.Search(SearchQuery.NotDeleted);
                var latestUids = uids.OrderByDescending(uid => uid.Id).Take(10).ToList();

                foreach (var uid in latestUids)
                {
                    var message = importantFolder.GetMessage(uid);
                    emails.Add(new
                    {
                        From = message.From.ToString(),
                        Subject = message.Subject,
                        Body = message.TextBody ?? "(Không có nội dung)",
                        Date = message.Date.ToString("g")
                    });
                }

                client.Disconnect(true);
            }
            catch (Exception ex)
            {
                return BadRequest($"Không thể lấy email trong Thư Quan Trọng. Lỗi: {ex.Message}");
            }

            return Ok(emails);
        }


        // Gửi email
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

                using var smtp = new MailKit.Net.Smtp.SmtpClient();
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
