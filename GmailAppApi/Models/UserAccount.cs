using System;

namespace GmailAppApi.Models
{
    public class UserAccount
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime LoginDate { get; set; }
    }
}
