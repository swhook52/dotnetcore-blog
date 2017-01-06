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
