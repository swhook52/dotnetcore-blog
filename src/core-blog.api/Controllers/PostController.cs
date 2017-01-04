using Business.Services;
using Dto;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ExampleCoreApi.Controllers
{
    [Route("api/[controller]")]
    public class PostController : Controller
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var posts = _postService.GetAll();
            return new ObjectResult(posts);
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            try
            {
                var post = _postService.Get(id);
                return new ObjectResult(post);
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

                return new CreatedResult(location, createdPost);
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
    }
}
