using System;
using Microsoft.AspNetCore.Mvc;
using Domain.Exceptions;
using Business.Services;

namespace core_blog.api.Controllers
{
    [Route("api/[controller]")]
    public class CommentController : Controller
    {
        private ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [AcceptVerbs("OPTIONS")]
        public IActionResult Options()
        {
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            Response.Headers.Add("Access-Control-Allow-Methods", "GET,POST,PUT,DELETE");
            return new StatusCodeResult(200);
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            try
            {
                var comments = _commentService.Get(id);
                return new OkObjectResult(comments);
            }
            catch (PostNotFoundException)
            {
                return new NotFoundResult();
            }
        }

        [HttpPost("{postId}")]
        public IActionResult Post(string postId, [FromBody]Dto.Comment comment)
        {
            try
            {
                var createdComment = _commentService.Create(postId, comment);
                var location = Url.RouteUrl(new { Action = "Get", Controller = "Comment", id = createdComment.Id });

                return new CreatedResult(location, createdComment);
            }
            catch (DuplicatePostException e)
            {
                return new BadRequestObjectResult(e.Message);
            }
            catch (Exception)
            {
                return new BadRequestResult();
            }
        }
        
        [HttpPut("{id}")]
        public IActionResult Put(Guid id, [FromBody]Dto.Comment comment)
        {
            if (comment == null || id == Guid.Empty)
                return new BadRequestResult();

            try
            {
                var editedComment = _commentService.Update(id, comment);
                return new OkObjectResult(editedComment);
            }
            catch (Exception)
            {
                return new BadRequestResult();
            }
        }
        
        [HttpDelete("{commentId}")]
        public IActionResult Delete(Guid commentId)
        {
            try
            {
                _commentService.Remove(commentId);
                return new NoContentResult();
            }
            catch (CommentNotFoundException e)
            {
                return new NotFoundObjectResult(e.Message);
            }
        }
    }
}
