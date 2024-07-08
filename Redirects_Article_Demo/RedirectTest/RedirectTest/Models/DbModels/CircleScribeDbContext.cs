using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;

namespace RedirectTest.Models.DbModels
{
    public class CircleScribeDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Article> Articles { get; set; }


        public CircleScribeDbContext(DbContextOptions<CircleScribeDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
