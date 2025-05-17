using System;
using System.Linq;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Search;
using MailKit;
using MimeKit;
using System.Net;

class EmailProgram
{
    static void Main()
    {
        // Thay email và app password của bạn ở đây
        string email = "nguyenhathanh844@gmail.com";
        string appPassword = "pwwskqgstpvmjhtp";

        Console.WriteLine("📧 CHƯƠNG TRÌNH GỬI & ĐỌC EMAIL BẰNG GMAIL + APP PASSWORD\n");

        Console.WriteLine("\nChọn chức năng:");
        Console.WriteLine("1. Gửi email");
        Console.WriteLine("2. Đọc 10 email mới nhất trong INBOX");
        Console.Write("Lựa chọn (1 hoặc 2): ");
        var choice = Console.ReadLine();

        if (choice == "1")
            SendEmail(email, appPassword);
        else if (choice == "2")
            ReadEmails(email, appPassword);
        else
            Console.WriteLine("❌ Lựa chọn không hợp lệ.");
    }

    static void ListFolders(string email, string appPassword)
    {
        using var client = new ImapClient();
        client.Connect("imap.gmail.com", 993, true);
        client.Authenticate(email, appPassword);

        var personal = client.GetFolder(client.PersonalNamespaces[0]);
        var folders = personal.GetSubfolders(false);

        foreach (var folder in folders)
        {
            Console.WriteLine($"- {folder.FullName}");
        }

        client.Disconnect(true);
    }

    static void SendEmail(string fromEmail, string appPassword)
    {
        Console.Write("\n📨 Email người nhận: ");
        string to = Console.ReadLine();

        Console.Write("📝 Tiêu đề: ");
        string subject = Console.ReadLine();


        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(fromEmail));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        //message.Body = new TextPart("plain") { Text = body };

        try
        {
            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate(fromEmail, appPassword);
            smtp.Send(message);
            smtp.Disconnect(true);

            Console.WriteLine("\n✅ Email đã được gửi thành công!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ Lỗi gửi mail: {ex.Message}");
        }
    }

    static void ReadEmails(string email, string appPassword)
    {
        try
        {
            using var client = new ImapClient();
            client.Connect("imap.gmail.com", 993, true);
            client.Authenticate(email, appPassword);

            var inbox = client.GetFolder("INBOX");
            inbox.Open(FolderAccess.ReadOnly);

            var uids = inbox.Search(SearchQuery.NotDeleted);
            var latestUids = uids.OrderByDescending(uid => uid.Id).Take(10).ToList();

            Console.WriteLine($"\n📥 Danh sách 10 email mới nhất trong INBOX:\n");

            foreach (var uid in latestUids)
            {
                var message = inbox.GetMessage(uid);

                Console.WriteLine($"📧 From: {message.From}");
                Console.WriteLine($"📝 Subject: {message.Subject}");
                //Console.WriteLine($"📄 Body:\n{message.TextBody ?? "(Không có nội dung văn bản)"}");
                Console.WriteLine(new string('-', 60));
            }

            client.Disconnect(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ Lỗi đọc email: {ex.Message}");
        }
    }
}
