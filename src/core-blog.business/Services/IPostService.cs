using System.Collections.Generic;
using Dto;

namespace Business.Services
{
    public interface IPostService
    {
        Post Get(string slug);
        Post Create(Post post);
        Post Update(string slug, Post post);
        IEnumerable<Post> GetAll();
    }
}
