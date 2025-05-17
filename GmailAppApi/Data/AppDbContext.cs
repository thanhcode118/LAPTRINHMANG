using GmailAppApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace GmailAppApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<UserAccount> UserAccounts { get; set; }
    }
}
