using System;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Domain.Exceptions;
using Business.Services;

namespace core_blog.api.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly IMapper _mapper;

        public CommentController(ICommentService commentService, IMapper mapper)
        {
            _commentService = commentService;
            _mapper = mapper;
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
                var comment = _commentService.Get(id);

                var dto = _mapper.Map<Domain.Comment, Dto.Comment>(comment);
                return new OkObjectResult(dto);
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

                var dto = _mapper.Map<Domain.Comment, Dto.Comment>(createdComment);
                return new CreatedResult(location, dto);
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

                var dto = _mapper.Map<Domain.Comment, Dto.Comment>(editedComment);
                return new OkObjectResult(dto);
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
