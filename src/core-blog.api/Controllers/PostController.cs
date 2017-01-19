﻿using Business.Services;
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

        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody]Post post)
        {
            try
            {
                var editedPost = _postService.Update(id, post);
                return new OkObjectResult(editedPost);
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
