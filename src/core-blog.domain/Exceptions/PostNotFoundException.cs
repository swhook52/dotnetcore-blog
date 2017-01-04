
namespace Domain.Exceptions
{
    public class PostNotFoundException : NotFoundException
    {
        public PostNotFoundException(string slug) : base("Post", "Slug", slug) { }
    }
}
