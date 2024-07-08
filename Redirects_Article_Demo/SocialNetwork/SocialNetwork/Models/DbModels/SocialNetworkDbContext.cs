using Microsoft.EntityFrameworkCore;

namespace SocialNetwork.Models.DbModels
{
    public class SocialNetworkDbContext : DbContext
    {
        public DbSet<Comment> Comments { get; set; }

        public SocialNetworkDbContext(DbContextOptions<SocialNetworkDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
