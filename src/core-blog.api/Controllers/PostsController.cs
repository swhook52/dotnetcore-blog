using Microsoft.AspNetCore.Mvc;
using Business.Services;

namespace core_blog.api.Controllers
{
    [Route("api/[controller]")]
    public class PostsController : Controller
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var posts = _postService.GetAll();
            return new OkObjectResult(posts);
        }
    }
}
