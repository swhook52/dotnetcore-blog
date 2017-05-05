using Microsoft.EntityFrameworkCore;

namespace Domain
{
    public class BloggingContext : DbContext
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<PostTag> PostTags { get; set; }

        public BloggingContext(DbContextOptions<BloggingContext> options) : base(options)
        {
        }
    }
}
