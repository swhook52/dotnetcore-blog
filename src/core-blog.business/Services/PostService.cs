using System;
using System.Collections.Generic;
using System.Linq;
using Domain;
using Domain.Exceptions;

namespace Business.Services
{
    public class PostService : IPostService
    {
        private readonly BloggingContext _context;

        public PostService(BloggingContext context)
        {
            _context = context;
        }

        public Dto.Post Get(string slug)
        {
            var post = _context.Posts.SingleOrDefault(p => p.Slug.ToLower() == slug.ToLower());
            if (post == null)
                throw new PostNotFoundException(slug);

            return new Dto.Post
            {
                Slug = post.Slug,
                Body = post.Body,
                DateCreated = post.DateCreated,
                DatePublished = post.DatePublished,
                IsFeatured = post.IsFeatured,
                IsStatic = post.IsStatic,
                Title = post.Title,
                Status = post.DatePublished == null ? Dto.PostStatus.Draft : Dto.PostStatus.Published
            };
        }

        public Dto.Post Create(Dto.Post post)
        {
            var slug = post.Slug?.ToLowerInvariant().Trim();
            if (string.IsNullOrEmpty(slug))
                throw new Exception("Slug is required");

            if (_context.Posts.Any(p => p.Slug == slug))
                throw new DuplicatePostException(slug);

            var newPost = new Post
            {
                Slug = slug,
                Body = post.Body,
                DateCreated = post.DateCreated ?? DateTime.UtcNow,
                DatePublished = post.DatePublished,
                IsFeatured = post.IsFeatured,
                IsStatic = post.IsStatic,
                Title = post.Title
            };

            _context.Posts.Add(newPost);
            _context.SaveChanges();

            return new Dto.Post
            {
                Slug = newPost.Slug,
                Body = newPost.Body,
                DateCreated = newPost.DateCreated,
                DatePublished = newPost.DatePublished,
                IsFeatured = newPost.IsFeatured,
                IsStatic = newPost.IsStatic,
                Title = newPost.Title,
                Status = newPost.DatePublished == null ? Dto.PostStatus.Draft : Dto.PostStatus.Published
            };
        }

        public IEnumerable<Dto.Post> GetAll()
        {
            return _context.Posts.Select(post => new Dto.Post
            {
                Slug = post.Slug,
                Body = post.Body,
                DateCreated = post.DateCreated,
                DatePublished = post.DatePublished,
                IsFeatured = post.IsFeatured,
                IsStatic = post.IsStatic,
                Title = post.Title,
                Status = post.DatePublished == null ? Dto.PostStatus.Draft : Dto.PostStatus.Published
            });
        }
    }
}
