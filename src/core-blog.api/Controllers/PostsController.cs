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

        [AcceptVerbs("OPTIONS")]
        public IActionResult Options()
        {
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            Response.Headers.Add("Access-Control-Allow-Methods", "GET");
            return new StatusCodeResult(200);
        }

        [HttpGet]
        public IActionResult Get()
        {
            var posts = _postService.GetAll();
            return new OkObjectResult(posts);
        }

        [HttpDelete]
        public IActionResult Delete()
        {
            _postService.DeleteAll();
            return new NoContentResult();
        }
    }
}
