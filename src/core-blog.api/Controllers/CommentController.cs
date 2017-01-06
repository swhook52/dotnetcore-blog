using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Domain.Exceptions;
using Business.Services;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

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

        // GET api/values/5
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

        // POST api/values
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
