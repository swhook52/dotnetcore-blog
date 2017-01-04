using Microsoft.EntityFrameworkCore;

namespace Domain
{
    public class BloggingContext : DbContext
    {
        public DbSet<Post> Posts { get; set; }

        public BloggingContext(DbContextOptions<BloggingContext> options) : base(options)
        {
        }
    }
}
