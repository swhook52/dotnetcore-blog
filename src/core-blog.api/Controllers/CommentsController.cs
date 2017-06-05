using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Domain.Exceptions;
using Business.Services;

namespace core_blog.api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CommentsController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly IMapper _mapper;

        public CommentsController(ICommentService commentService, IMapper mapper)
        {
            _commentService = commentService;
            _mapper = mapper;
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

                var dtos = _mapper.Map<Domain.Comment[], Dto.Comment[]>(comments);
                return new OkObjectResult(dtos);
            }
            catch (PostNotFoundException)
            {
                return new NotFoundResult();
            }
        }
    }
}
