using Business.Services;
using Dto;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System;
using AutoMapper;

namespace core_blog.api.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IMapper _mapper;

        public PostController(IPostService postService, IMapper mapper)
        {
            _postService = postService;
            _mapper = mapper;
        }

        [AcceptVerbs("OPTIONS")]
        public IActionResult Options()
        {
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            Response.Headers.Add("Access-Control-Allow-Methods", "GET,POST,PUT");
            return new StatusCodeResult(200);
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            try
            {
                var post = _postService.Get(id);
                var dto = _mapper.Map<Domain.Post, Post>(post);
                return new ObjectResult(dto);
            }
            catch (PostNotFoundException)
            {
                return new NotFoundResult();
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody]Post post)
        {
            try
            {
                var createdPost = _postService.Create(post);
                var location = Url.RouteUrl(new { Action = "Get", Controller = "Post", id = createdPost.Slug });

                var dto = _mapper.Map<Domain.Post, Post>(createdPost);

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
        public IActionResult Put(string id, [FromBody]Post post)
        {
            try
            {
                var editedPost = _postService.Update(id, post);
                var dto = _mapper.Map<Domain.Post, Post>(editedPost);

                return new OkObjectResult(dto);
            }
            catch (Exception)
            {
                return new BadRequestResult();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            try
            {
                _postService.Delete(id);
                return new NoContentResult();
            }
            catch (Exception)
            {
                return new BadRequestResult();
            }

        }
    }
}
