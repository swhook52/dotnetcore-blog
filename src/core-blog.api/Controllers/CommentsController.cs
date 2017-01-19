using Microsoft.AspNetCore.Mvc;
using Domain.Exceptions;
using Business.Services;

namespace core_blog.api.Controllers
{
    [Route("api/[controller]")]
    public class CommentsController : Controller
    {
        private ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [AcceptVerbs("OPTIONS")]
        public IActionResult Options()
        {
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            Response.Headers.Add("Access-Control-Allow-Methods", "GET");
            return new StatusCodeResult(200);
        }

        [HttpGet("{postId}")]
        public IActionResult Get(string postId)
        {
            try
            {
                var comments = _commentService.GetAll(postId);
                return new OkObjectResult(comments);
            }
            catch (PostNotFoundException)
            {
                return new NotFoundResult();
            }
        }
    }
}
