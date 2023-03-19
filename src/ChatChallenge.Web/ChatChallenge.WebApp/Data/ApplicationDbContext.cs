using ChatChallenge.WebApp.Data.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChatChallenge.WebApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<ChatGroupMessage> ChatGroupMessages { get; set; }
        public DbSet<OnlineUserInGroup> OnlineUserInGroups { get; set; }

        internal void Where()
        {
            throw new NotImplementedException();
        }
    }
}